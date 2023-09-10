using RabbitMQ.Client;
using Silver.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.RabbitMQ
{
    /// <summary>
    /// 生产者-消息发布者
    /// </summary>
    public class RabbitMQPublisher
    {
        public IConnection connection;
        public IModel channel;

        public RabbitMQPublisher()
        {
            var factory = new ConnectionFactory()
            {
                HostName = ConfigurationUtil.GetSection("RabbitMQ:IP"),//IP地址
                Port = ConfigurationUtil.GetSection("RabbitMQ:Port").ToInt(),//端口号
                UserName = ConfigurationUtil.GetSection("RabbitMQ:UserName"),//用户账号
                Password = ConfigurationUtil.GetSection("RabbitMQ:PassWord") //用户密码
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public RabbitMQPublisher(string ipAddress, int port, string username, string password)
        {
            var factory = new ConnectionFactory()
            {
                HostName = ipAddress,//IP地址
                Port = port,//端口号
                UserName = username,//用户账号
                Password = password//用户密码
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="channelName">渠道</param>
        public bool Publish(string message, string channelName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("消息不能为空");
            }
            if (!RabbitMQSetting.ListRabbitMQ.Contains(channelName))
            {
                var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };
                channel.QueueDeclare(channelName, durable: durable, exclusive: exclusive, autoDelete: autoDelete, arguments: arguments);
                RabbitMQSetting.ListRabbitMQ.Add(channelName);
            }
            channel.BasicPublish(exchange: "", routingKey: channelName, basicProperties: null, body: Encoding.UTF8.GetBytes(message));
            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public bool Publish<T>(T message, string channelName, bool durable = true, bool exclusive = false, bool autoDelete = false) where T : class
        {
            if (message == null)
            {
                throw new Exception("消息不能为空");
            }
            if (!RabbitMQSetting.ListRabbitMQ.Contains(channelName))
            {
                var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };
                channel.QueueDeclare(channelName, durable: durable, exclusive: exclusive, autoDelete: autoDelete, arguments: arguments);
                RabbitMQSetting.ListRabbitMQ.Add(channelName);
            }
            channel.BasicPublish(exchange: "", routingKey: channelName, basicProperties: null, body: Encoding.UTF8.GetBytes(message.ToJson()));
            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public bool Publish<T>(T message, bool durable = true, bool exclusive = false, bool autoDelete = false) where T : class
        {
            if (message == null)
            {
                throw new Exception("消息不能为空");
            }
            var channelName = typeof(T).Name;
            if (!RabbitMQSetting.ListRabbitMQ.Contains(channelName))
            {
                var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };
                channel.QueueDeclare(channelName, durable: durable, exclusive: exclusive, autoDelete: autoDelete, arguments: arguments);
                RabbitMQSetting.ListRabbitMQ.Add(channelName);
            }
            channel.BasicPublish(exchange: "", routingKey: channelName, basicProperties: null, body: Encoding.UTF8.GetBytes(message.ToJson()));
            return true;
        }

        /// <summary>
        /// 消息分发
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool PublishFanOut(string message, string exchange, string routingKey = "", string type = "fanout")
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("消息不能为空");
            }
            //4. 声明信息交换机
            channel.ExchangeDeclare(exchange: exchange, type: type);

            //5. 构建字节数据包 
            var body = Encoding.UTF8.GetBytes(message);

            //6. 发布到指定exchange，fanout类型的会忽视routingKey的值，所以无需填写
            channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);
            return true;
        }

        /// <summary>
        /// 消息分发
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool PublishFanOut<T>(T message, string exchange, string routingKey = "", string type = "fanout")
        {
            if (message == null)
            {
                throw new Exception("消息不能为空");
            }
            //4. 声明信息交换机
            channel.ExchangeDeclare(exchange: exchange, type: type);

            //5. 构建字节数据包 
            var body = Encoding.UTF8.GetBytes(message.ToJson());

            //6. 发布到指定exchange，fanout类型的会忽视routingKey的值，所以无需填写
            channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);
            return true;
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void CloseAt()
        {
            if (channel != null)
            {
                if (channel.IsOpen)
                {
                    channel.Close();
                }
                channel.Abort();
                channel.Dispose();
            }

            if (connection != null)
            {
                if (connection.IsOpen)
                {
                    connection.Close();
                }
            }
        }


    }
}
