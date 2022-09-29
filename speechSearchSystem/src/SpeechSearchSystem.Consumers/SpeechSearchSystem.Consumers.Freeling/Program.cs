using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using SpeechSearchSystem.Consumers.Freeling;
using SpeechSearchSystem.Contracts;

await Task.Delay(15000);
var factory = new ConnectionFactory
{
    HostName = "api-rabbitmq",
    UserName = "myuser",
    Password = "mypassword"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare("speeches", exclusive: false, durable: true, autoDelete: false);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine("Waiting for messages");
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (sender, args) =>
{
    try
    {
        var body = args.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var speechCreated = JsonSerializer.Deserialize<CreatedSpeechMessage>(json);
        if (speechCreated is null)
        {
            Console.WriteLine("Message is null");
            return;
        }

        Console.WriteLine($"Speech {speechCreated.Id}");
        var freeling = new FreelingService("freeling", 50005);
        var morphologicalResult = await freeling.ProcessMorphologicalAnalysisAsync(speechCreated.Text);

        using var httpClient = new HttpClient();
        var response = await httpClient
            .PutAsync(
                $"http://speech-search-system:80/api/speech/{speechCreated.Id}",
                new StringContent(
                    JsonSerializer.Serialize(new UpdateSpeechCommand(speechCreated.Id, morphologicalResult)),
                    Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"{response.StatusCode} - {speechCreated.Id}");
            return;
        }

        ((EventingBasicConsumer)sender!).Model.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
};
channel.BasicConsume(queue: "speeches", autoAck: false, consumer: consumer);

Console.ReadLine();

internal record UpdateSpeechCommand(string Id, string MorphologicalAnalysisRaw);