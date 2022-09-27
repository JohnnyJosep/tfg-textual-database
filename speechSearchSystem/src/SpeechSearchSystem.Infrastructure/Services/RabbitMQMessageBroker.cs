using System.Text;
using System.Text.Json;

using RabbitMQ.Client;

using SpeechSearchSystem.Application.Services;

namespace SpeechSearchSystem.Infrastructure.Services;

internal class RabbitMqMessageBroker : IMessageBroker
{
    public void SendMessage<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = "api-rabbitmq",
            UserName = "myuser", 
            Password = "mypassword"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("speeches", exclusive: false, durable: true, autoDelete: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(exchange: string.Empty, routingKey: "speeches", basicProperties: properties, body: body);
    }
}