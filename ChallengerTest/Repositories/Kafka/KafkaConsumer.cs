using Confluent.Kafka;

namespace ChallengerTest.Repositories.Kafka
{
    public class KafkaConsumer
    {
        private readonly string _topic = "operations";
        private readonly string _bootstrapServers = "localhost:9092";
        private readonly string _groupId = "permission-group";

        public void ConsumeMessages()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            Console.WriteLine("Listening to Kafka topic...");
            while (true)
            {
                var result = consumer.Consume();
                Console.WriteLine($"Received message: {result.Value}");
            }
        }
    }
}