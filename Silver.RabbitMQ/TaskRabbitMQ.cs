using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Silver.Basic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Silver.RabbitMQ
{

    /// <summary>
    /// 多线程 RabbitMQ 消费端
    /// </summary>
    public class TaskRabbitMQ
    {

        private string rabbitHostName = "127.0.0.1";
        private int rabbitPort = 5672;
        private string rabbitUserName = "admin";
        private string rabbitPassWord = "admin";
        private int rabbitTask = 1;
        private bool isConsule = false;

        public TaskRabbitMQ(int task = 1, bool consule = false)
        {
            this.rabbitHostName = ConfigurationUtil.GetSection("RabbitMQ:IP");//IP地址
            this.rabbitPort = ConfigurationUtil.GetSection("RabbitMQ:Port").ToInt();//端口号
            this.rabbitUserName = ConfigurationUtil.GetSection("RabbitMQ:UserName");//用户账号
            this.rabbitPassWord = ConfigurationUtil.GetSection("RabbitMQ:PassWord");//用户密码 
            this.rabbitTask = task;//线程数量
            this.isConsule = consule;
        }

        public TaskRabbitMQ(string ip, int port, string userName, string passWord, int task = 1, bool consule = false)
        {
            this.rabbitHostName = ip;//IP地址
            this.rabbitPort = port;//端口号
            this.rabbitUserName = userName;//用户账号
            this.rabbitPassWord = passWord;//用户密码
            this.rabbitTask = task;//线程数量
            this.isConsule = consule;
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="channelName">队列名称</param>
        /// <param name="callback">回调函数 ，是否消费后保留</param>
        /// <param name="durable">该队列是否应该在代理重新启动后继续存在</param>
        /// <param name="exclusive">此队列的使用是否应限于其声明连接？这样的队列将在其声明连接关闭时被删除。</param>
        /// <param name="autoDelete">当最后一个消费者（如果有）取消订阅时，是否应自动删除此队列？</param>
        public void Subscribe(string channelName, Action<string, Action<bool>> callback, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            List<Task> tasks = new List<Task>();
            Random rdowm = new Random();
            for (int i = 1; i <= this.rabbitTask; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    int taskNo = rdowm.Next(1, int.MaxValue);
                    ResetConnection:
                    try
                    {
                        ConsuleLog($"启动RabbitMQ任务：{taskNo}");
                        var factory = new ConnectionFactory()
                        {
                            HostName = this.rabbitHostName, //IP地址
                            Port = this.rabbitPort, //端口号
                            UserName = this.rabbitUserName, //用户账号
                            Password = this.rabbitPassWord //用户密码
                        };
                        IConnection connection = factory.CreateConnection();
                        IModel channel = connection.CreateModel();
                        channel.QueueDeclare(channelName, durable, exclusive, autoDelete, null);
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, e) =>
                        {
                            var message = Encoding.UTF8.GetString(e.Body.ToArray());
                            callback(message, (bool isAck) =>
                            {
                                channel.BasicAck(e.DeliveryTag, isAck);
                            });
                        };
                        channel.BasicConsume(queue: channelName, autoAck: false, consumer: consumer);
                    }
                    catch (Exception ex)
                    {
                        ConsuleLog($"启动RabbitMQ失败：{taskNo}  原因：{ex.Message}");
                        Thread.Sleep(10);
                        goto ResetConnection;
                    }
                }));
            }
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="channelName">队列名称</param>
        /// <param name="callback">回调函数 ，是否消费后保留</param>
        /// <param name="durable">该队列是否应该在代理重新启动后继续存在</param>
        /// <param name="exclusive">此队列的使用是否应限于其声明连接？这样的队列将在其声明连接关闭时被删除。</param>
        /// <param name="autoDelete">当最后一个消费者（如果有）取消订阅时，是否应自动删除此队列？</param>
        public void Subscribe<T>(string channelName, Action<T, Action<bool>> callback, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            List<Task> tasks = new List<Task>();
            Random rdowm = new Random();
            for (int i = 1; i <= this.rabbitTask; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    int taskNo = rdowm.Next(1, int.MaxValue);
                    ResetConnection:
                    try
                    {
                        ConsuleLog($"启动RabbitMQ任务：{taskNo}");
                        var factory = new ConnectionFactory()
                        {
                            HostName = this.rabbitHostName, //IP地址
                            Port = this.rabbitPort, //端口号
                            UserName = this.rabbitUserName, //用户账号
                            Password = this.rabbitPassWord //用户密码
                        };
                        IConnection connection = factory.CreateConnection();
                        IModel channel = connection.CreateModel();
                        channel.QueueDeclare(channelName, durable, exclusive, autoDelete, null);
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, e) =>
                        {
                            var message = Encoding.UTF8.GetString(e.Body.ToArray());
                            callback(message.JsonToObject<T>(), (bool isAck) =>
                            {
                                channel.BasicAck(e.DeliveryTag, isAck);
                            });
                        };
                        channel.BasicConsume(queue: channelName, autoAck: false, consumer: consumer);
                    }
                    catch (Exception ex)
                    {
                        ConsuleLog($"启动RabbitMQ失败：{taskNo}  原因：{ex.Message}");
                        Thread.Sleep(10);
                        goto ResetConnection;
                    }
                }));
            }
        }


        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="message"></param>
        private void ConsuleLog(string message)
        {
            if (this.isConsule == true)
            {
                Console.WriteLine(message);
            }
        }

    }
}
