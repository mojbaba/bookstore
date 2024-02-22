namespace BookStore.Authentication.Jwt.KafkaLoggedOut;

public class KafkaUserLoggedOutOptions
{
    public string Topic { get; set; }
    public string GroupId { get; set; }
    public string BootstrapServers { get; set; }
}