namespace BookStore.EventLog.Kafka;

public class KafkaOptions
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public TopicsConfiguration Topics { get; set; }
}

public class TopicsConfiguration
{
    public string UserLogoutTopic { get; set; }
    public string UserLoginTopic { get; set; }
    public string UserRegisterTopic { get; set; }
    public string OrderFailedTopic { get; set; }
    public string OrderCreatedTopic { get; set; }
    public string BalanceDeductedTopic { get; set; }
    public string BalanceDeductionFailedTopic { get; set; }
    public string BooksPackedTopic { get; set; }
    public string BooksPackingFailedTopic { get; set; }
}
