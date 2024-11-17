using Confluent.Kafka;
using System.Text.Json;

namespace ChallengerTest.Repositories.Kafka
{
    public class KafkaProducer
    {
        private readonly string _topic = "operations";
        private readonly string _bootstrapServers = "localhost:9092";

        public async Task SendMessageAsync(OperationMessage message)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            string jsonMessage = JsonSerializer.Serialize(message);

            try
            {
                var result = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
                Console.WriteLine($"Message delivered to {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}