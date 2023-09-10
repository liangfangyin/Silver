using Confluent.Kafka;
using Newtonsoft.Json;
using Silver.Basic;
using System;
using System.Threading.Tasks;

namespace Silver.Kafka
{
    /// <summary>
    /// 生产者-消息发布者
    /// </summary>
    public class KafkaMQPublisher
    {

        private IProducer<string, string> producer;
        public KafkaMQPublisher()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = ConfigurationUtil.GetSection("Kafka:Server"),
                BatchSize = 16384, // 修改批次大小为16K
                LingerMs = 0 // 修改等待时间为20ms
            };
            if (!string.IsNullOrEmpty(ConfigurationUtil.GetSection("Kafka:UserName")))
            {
                config.SaslUsername = ConfigurationUtil.GetSection("Kafka:UserName");
            }
            if (!string.IsNullOrEmpty(ConfigurationUtil.GetSection("Kafka:PassWord")))
            {
                config.SaslPassword = ConfigurationUtil.GetSection("Kafka:PassWord");
            }
            producer = new ProducerBuilder<string, string>(config).Build();
        }

        public KafkaMQPublisher(string server, string username = "", string password = "")
        {
            var config = new ProducerConfig
            {
                BootstrapServers = server,
                BatchSize = 16384, // 修改批次大小为16K
                LingerMs = 0 // 修改等待时间为20ms
            };
            //var factory = new ProducerConfig
            //{
            //    BootstrapServers = AppSettings.KafkaServer,
            //    SaslMechanism = SaslMechanism.Plain,
            //    SecurityProtocol = SecurityProtocol.SaslSsl,
            //};
            if (!string.IsNullOrEmpty(username))
            {
                config.SaslUsername = username;
            }
            if (!string.IsNullOrEmpty(password))
            {
                config.SaslPassword = password;
            }
            producer = new ProducerBuilder<string, string>(config).Build();
        }

        /// <summary>
        /// 消息推送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync<T>(string topicName, T message, string groupBy = "") where T : class
        {
            if (string.IsNullOrEmpty(groupBy))
            {
                groupBy = Guid.NewGuid().ToString();
            }
            await producer.ProduceAsync(topicName, new Message<string, string>
            {
                Key = groupBy,
                Value = JsonConvert.SerializeObject(message)
            }); ;
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            if (producer != null)
            {
                producer.Dispose();
            }
        }

    }
}
