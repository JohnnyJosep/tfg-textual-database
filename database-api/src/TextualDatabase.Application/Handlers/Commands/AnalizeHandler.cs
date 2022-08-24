using System.Net;
using System.Net.Sockets;
using System.Text;

using MediatR;

namespace TextualDatabase.Application.Handlers.Commands;

public record AnalizeCommand(string Text) : IRequest<string>;

internal class AnalizeHandler : IRequestHandler<AnalizeCommand, string>
{
    private const string ResetStats = "RESET_STATS";
    private const string FlushBuffer = "FLUSH_BUFFER";
    
    private const char EndOfTransmision =  '\u0000';

    private const string ServerReady = "FL-SERVER-READY";

    private readonly Socket _socket;
    //private NetworkStream _ns;

    public AnalizeHandler()
    {
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            LingerState = new LingerOption(true, 10),
            SendTimeout = 500,
            ReceiveTimeout = 500
        };
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
    }

    public async Task<string> Handle(AnalizeCommand request, CancellationToken cancellationToken)
    {

        var host = Dns.GetHostEntry("freeling");
        var ipAddress = host.AddressList[0];
        var endpoint = new IPEndPoint(ipAddress, 50005);

        await _socket.ConnectAsync(endpoint, cancellationToken);
        //_ns = new NetworkStream(_socket);

        await SendMessageAsync(ResetStats, cancellationToken);
        var serverReadyResponse = await ReceiveMessageAsync(cancellationToken);
        if (serverReadyResponse != ServerReady)
            throw new Exception();

        await SendMessageAsync(FlushBuffer, cancellationToken);
        await ReceiveMessageAsync(cancellationToken);

        await SendMessageAsync(request.Text, cancellationToken);
        var response = await ReceiveMessageAsync(cancellationToken);

        await SendMessageAsync(FlushBuffer, cancellationToken);
        await ReceiveMessageAsync(cancellationToken);

        return response;
    }

    private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        //await _socket.SendAsync(Encoding.UTF8.GetBytes(message), cancellationToken);
        //await _socket.SendAsync(new[] { EndOfTransmision }, cancellationToken);
        //// flush??

        using var stream = new NetworkStream(_socket);
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message + EndOfTransmision), cancellationToken);
        await stream.FlushAsync(cancellationToken);

        await Task.Delay(1000);
    }

    private async Task<string> ReceiveMessageAsync(CancellationToken cancellationToken, int maxIntents = 5)
    {
        var buffer = new byte[2048];
        var builder = new StringBuilder();
        int bytesReceived;
        
        /*do
        {
            bytesReceived = await _socket.ReceiveAsync(buffer, cancellationToken);
            if (bytesReceived > 0) builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
        } while (bytesReceived > 0 && buffer[bytesReceived - 1] != EndOfTransmision);
        
        return builder.ToString().TrimEnd('\u0000');*/

        using var stream = new NetworkStream(_socket);
        if (stream.CanRead && stream.DataAvailable)
        {
            do
            {
                bytesReceived = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesReceived > 0) builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
            } while (bytesReceived > 0 && buffer[bytesReceived - 1] != EndOfTransmision);

            await stream.FlushAsync(cancellationToken);
            return builder.ToString().TrimEnd(EndOfTransmision);
        }

        if (maxIntents > 0)
        {
            await Task.Delay(1000);
            return await ReceiveMessageAsync(cancellationToken, maxIntents--);
        }

        return string.Empty;
    }
}
