using Confluent.Kafka;
using Newtonsoft.Json;
using Silver.Basic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Silver.Kafka
{
    /// <summary>
    /// 消费者-消息订阅者/消费者
    /// </summary>
    public class KafkaMQSubscriber
    {
        private IConsumer<Ignore, string> consumer;

        public KafkaMQSubscriber(string groupID = "")
        {
            if (string.IsNullOrEmpty(groupID))
            {
                groupID = "Consumer";
            }
            var config = new ConsumerConfig
            {
                BootstrapServers = ConfigurationUtil.GetSection("Kafka:Server"),
                GroupId = groupID,
                EnableAutoCommit = false, // 禁止AutoCommit
                Acks = Acks.Leader, // 假设只需要Leader响应即可
                AutoOffsetReset = AutoOffsetReset.Earliest // 从最早的开始消费起
            };
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }


        public KafkaMQSubscriber(string server, string username, string password, string groupID = "")
        {
            if (string.IsNullOrEmpty(groupID))
            {
                groupID = "Consumer";
            }
            var config = new ConsumerConfig
            {
                BootstrapServers = ConfigurationUtil.GetSection("Kafka:Server"),
                GroupId = groupID,
                EnableAutoCommit = false, // 禁止AutoCommit
                Acks = Acks.Leader, // 假设只需要Leader响应即可
                AutoOffsetReset = AutoOffsetReset.Earliest // 从最早的开始消费起
            };
            //var factory = new ConsumerConfig
            //{
            //    GroupId = GroupID,
            //    BootstrapServers = server,
            //    AutoOffsetReset = AutoOffsetReset.Earliest,
            //    SaslMechanism = SaslMechanism.Plain,
            //    SecurityProtocol = SecurityProtocol.SaslSsl,
            //};
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topics"></param>
        /// <param name="messageFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SubscribeAsync<T>(IEnumerable<string> topics, Action<T> messageFunc, CancellationToken cancellationToken = default) where T : class
        {
            consumer.Subscribe(topics);
            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        Console.WriteLine($"Consumed message '{consumeResult.Message?.Value}' at: '{consumeResult?.TopicPartitionOffset}'.");
                        if (consumeResult.IsPartitionEOF)
                        {
                            Console.WriteLine($" - {DateTime.Now:yyyy-MM-dd HH:mm:ss} 已经到底了：{consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                            continue;
                        }
                        T messageResult = null;
                        try
                        {
                            messageResult = JsonConvert.DeserializeObject<T>(consumeResult.Message.Value);
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = $" - {DateTime.Now:yyyy-MM-dd HH:mm:ss}【Exception 消息反序列化失败，Value：{consumeResult.Message.Value}】 ：{ex.StackTrace?.ToString()}";
                            Console.WriteLine(errorMessage);
                            messageResult = null;
                        }
                        if (messageResult != null/* && consumeResult.Offset % commitPeriod == 0*/)
                        {
                            messageFunc(messageResult);
                            try
                            {
                                consumer.Commit(consumeResult);
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                consumer.Close();
            }
            await Task.CompletedTask;
        }


        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            if (consumer != null)
            {
                consumer.Close();
                consumer.Dispose();
            }
        }


    }
}
