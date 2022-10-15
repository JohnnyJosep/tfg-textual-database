using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using SpeechSearchSystem.Consumers.Freeling;
using SpeechSearchSystem.Contracts;


var rabbitHost = "api-rabbitmq";

var freelingHost = "freeling";
var freelingPort = 50005;

var text = "Señor presidente, usted sabe que no hay turnos por alusiones en este preciso turno...";
//text = text.Replace("...", "…");
text = text.Replace("...", ".");


using var fl = new FreelingService("localhost", freelingPort);
var response =
    await fl.ProcessMorphologicalAnalysisAsync(text);

Console.WriteLine(text);
Console.WriteLine(response);

/*

Console.WriteLine("START");
await Task.Delay(10000);

var factory = new ConnectionFactory
{
    HostName = rabbitHost,
    UserName = "myuser",
    Password = "mypassword"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var cancellationTokenSource = new CancellationTokenSource();
CancellationToken cancellationToken = cancellationTokenSource.Token;

channel.QueueDeclare("speeches", exclusive: false, durable: true, autoDelete: false);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine("Waiting for messages");
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (sender, args) =>
{
    try
    {
        var body = args.Body.ToArray();
        var speechCreated = JsonSerializer.Deserialize<CreatedSpeechMessage>(Encoding.UTF8.GetString(body));
        if (speechCreated is null)
        {
            Console.WriteLine("Message is null");
            return;
        }

        Console.WriteLine($"\n\n\nSpeech {speechCreated.Id}");
        using var freeling = new FreelingService(freelingHost, freelingPort);
        var morphologicalResult = await freeling.ProcessMorphologicalAnalysisAsync(speechCreated.Text);

        using var httpClient = new HttpClient();
        var json = JsonSerializer.Serialize(new UpdateSpeechCommand(speechCreated.Id, morphologicalResult));
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient
            .PutAsync($"http://speech-search-system:80/api/speech/{speechCreated.Id}", content);
        response.EnsureSuccessStatusCode();

        ((EventingBasicConsumer)sender!).Model.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
    }
    catch (Exception ex)
    {
        ((EventingBasicConsumer)sender!).Model.BasicNack(deliveryTag: args.DeliveryTag, multiple: false, requeue: true);

        Console.WriteLine($"\t{ex.Message}");
    }
};
channel.BasicConsume(queue: "speeches", autoAck: false, consumer: consumer);

Console.ReadLine();

cancellationTokenSource.Cancel();
WaitHandle.WaitAny(new[] { cancellationToken.WaitHandle });

channel.Close();
channel.Dispose();
connection.Close();
connection.Dispose();

Console.WriteLine("END");

internal record UpdateSpeechCommand(string Id, string MorphologicalAnalysisRaw);
*/