namespace ChallengerTest.Repositories.Kafka
{
    public class OperationMessage
    {
        public Guid Id { get; set; }
        public required string NameOperation { get; set; } // "modify", "request", or "get"
    }

}