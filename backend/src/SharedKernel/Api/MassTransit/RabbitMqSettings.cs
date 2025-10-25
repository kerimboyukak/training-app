namespace Api.MassTransit;

public class RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;    // location of the event bus
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;   // the queue that holds the messages/events that are to be processed
}