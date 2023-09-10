using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Silver.Basic;
using System;
using System.Text;

namespace Silver.RabbitMQ
{
    /// <summary>
    /// 消费者-消息订阅者/消费者
    /// </summary>
    public class RabbitMQSubscriber
    {
        private ConnectionFactory factory;
        public RabbitMQSubscriber()
        {
            if (RabbitMQSetting.connection == null)
            {
                factory = new ConnectionFactory()
                {
                    HostName = ConfigurationUtil.GetSection("RabbitMQ:IP"),//IP地址
                    Port = ConfigurationUtil.GetSection("RabbitMQ:Port").ToInt(),//端口号
                    UserName = ConfigurationUtil.GetSection("RabbitMQ:UserName"),//用户账号
                    Password = ConfigurationUtil.GetSection("RabbitMQ:PassWord") //用户密码
                };
                RabbitMQSetting.connection = factory.CreateConnection();
                RabbitMQSetting.channel = RabbitMQSetting.connection.CreateModel();
            } 
        }

        public RabbitMQSubscriber(string ipAddress, int port, string username, string password)
        {
            if (RabbitMQSetting.connection == null)
            {
                factory = new ConnectionFactory()
                {
                    HostName = ipAddress,//IP地址
                    Port = port,//端口号
                    UserName = username,//用户账号
                    Password = password//用户密码
                };
                RabbitMQSetting.connection = factory.CreateConnection();
                RabbitMQSetting.channel = RabbitMQSetting.connection.CreateModel();
            }
        }

        /// <summary>
        /// 消费消息，并执行回调。
        /// </summary>
        /// <param name="channelName">渠道名称</param>
        /// <param name="callback">返回函数</param>
        /// <param name="durable">该队列是否应该在代理重新启动后继续存在</param>
        /// <param name="exclusive">此队列的使用是否应限于其声明连接？这样的队列将在其声明连接关闭时被删除。</param>
        /// <param name="autoDelete">当最后一个消费者（如果有）取消订阅时，是否应自动删除此队列？</param>
        public void Subscribe(string channelName, Action<string,ulong> callback, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            OpenConnection();
            //5. 构造消费者实例
            var consumer = new EventingBasicConsumer(RabbitMQSetting.channel);
            //6. 绑定消息接收后的事件委托
            consumer.Received += (model, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                callback(message, e.DeliveryTag);
                if (autoDelete)
                {
                    RabbitMQSetting.channel.BasicAck(e.DeliveryTag, false);
                }
            };
            //7. 启动消费者
            RabbitMQSetting.channel.BasicConsume(queue: channelName,autoAck: false,consumer: consumer);
        }

        /// <summary>
        /// 消费消息，并执行回调。
        /// </summary>
        /// <param name="channelName">渠道名称</param>
        /// <param name="callback">返回函数</param>
        /// <param name="durable">该队列是否应该在代理重新启动后继续存在</param>
        /// <param name="exclusive">此队列的使用是否应限于其声明连接？这样的队列将在其声明连接关闭时被删除。</param>
        /// <param name="autoDelete">当最后一个消费者（如果有）取消订阅时，是否应自动删除此队列？</param>
        public void Subscribe<T>(string channelName, Action<T, ulong> callback, bool durable = true, bool exclusive = false, bool autoDelete = false) where T : class
        {
            OpenConnection();
            //5. 构造消费者实例
            var consumer = new EventingBasicConsumer(RabbitMQSetting.channel);
            //6. 绑定消息接收后的事件委托
            consumer.Received += (model, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                callback(message.JsonToObject<T>(), e.DeliveryTag);
                if (autoDelete)
                {
                    RabbitMQSetting.channel.BasicAck(e.DeliveryTag, false);
                }
            };
            //7. 启动消费者
            RabbitMQSetting.channel.BasicConsume(queue: channelName, autoAck: false, consumer: consumer);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="callback"></param>
        /// <param name="autoDelete"></param>
        /// <param name="type"></param>
        public void SubscribeFanOut(string exchange, Action<string, ulong> callback,bool autoDelete=false, string type= "fanout")
        {
            OpenConnection();
            //4. 声明信息交换机
            RabbitMQSetting.channel.ExchangeDeclare(exchange: exchange, type: type);
            //生成随机队列名称
            var queueName = RabbitMQSetting.channel.QueueDeclare().QueueName;
            //绑定队列到指定fanout类型exchange
            RabbitMQSetting.channel.QueueBind(queue: queueName, exchange: exchange,routingKey: "");
            //5. 构造消费者实例
            var consumer = new EventingBasicConsumer(RabbitMQSetting.channel);
            //6. 绑定消息接收后的事件委托
            consumer.Received += (model, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                callback(message, e.DeliveryTag);
                if (autoDelete)
                {
                    RabbitMQSetting.channel.BasicAck(e.DeliveryTag, false);
                }
            };
            RabbitMQSetting.channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="callback"></param>
        /// <param name="autoDelete"></param>
        /// <param name="type"></param>
        public void SubscribeFanOut<T>(string exchange, Action<T, ulong> callback, bool autoDelete = false, string type = "fanout")
        {
            OpenConnection();
            //4. 声明信息交换机
            RabbitMQSetting.channel.ExchangeDeclare(exchange: exchange, type: type);
            //生成随机队列名称
            var queueName = RabbitMQSetting.channel.QueueDeclare().QueueName;
            //绑定队列到指定fanout类型exchange
            RabbitMQSetting.channel.QueueBind(queue: queueName, exchange: exchange, routingKey: "");
            //5. 构造消费者实例
            var consumer = new EventingBasicConsumer(RabbitMQSetting.channel);
            //6. 绑定消息接收后的事件委托
            consumer.Received += (model, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                callback(message.JsonToObject<T>(), e.DeliveryTag);
                if (autoDelete)
                {
                    RabbitMQSetting.channel.BasicAck(e.DeliveryTag, false);
                }
            };
            RabbitMQSetting.channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }


        private void OpenConnection()
        {
            if (RabbitMQSetting.channel.IsOpen == false)
            {
                RabbitMQSetting.connection = factory.CreateConnection();
                RabbitMQSetting.channel = RabbitMQSetting.connection.CreateModel();
            }
        }

        public void CloseAt()
        {
            if (RabbitMQSetting.channel != null)
            {
                RabbitMQSetting.channel.Abort();
                if (RabbitMQSetting.channel.IsOpen)
                    RabbitMQSetting.channel.Close();

                RabbitMQSetting.channel.Dispose();
            }

            if (RabbitMQSetting.channel != null)
            {
                if (RabbitMQSetting.channel.IsOpen)
                    RabbitMQSetting.channel.Close();

                RabbitMQSetting.channel.Dispose();
            }
        }


    }
}
