using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SpeechSearchSystem.Consumers.Freeling;

internal class FreelingService : IDisposable
{
    private const string MessageResetStats = "RESET_STATS";
    private const string MessageFlushBuffer = "FLUSH_BUFFER";
    private const string MessageServerReady = "FL-SERVER-READY";
    private const byte Zero = (byte)0;

    private readonly IPEndPoint _ipEndPoint;
    private readonly Socket _socket;

    public FreelingService(string host, int port)
    {
        var ipHostInfo = Dns.GetHostEntry(host);
        var ipAddress = ipHostInfo.AddressList[0];
        _ipEndPoint = new IPEndPoint(ipAddress, port);
        _socket = new Socket(
            _ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
        _socket.LingerState = new LingerOption(true, 10);
    }

    public async Task<string> ProcessMorphologicalAnalysisAsync(string text)
    {
        await _socket.ConnectAsync(_ipEndPoint);
        await WriteAsync(MessageResetStats);
        var ready = await ReadAsync();
        if (ready != MessageServerReady)
        {
            throw new Exception("Server not ready");
        }

        await WriteAsync(MessageFlushBuffer);
        await ReadAsync();

        await WriteAsync(text);
        var response = await ReadAsync();
        
        await WriteAsync(MessageFlushBuffer);
        await ReadAsync();
        return response;
    }

    private async Task WriteAsync(string message)
    {
        await _socket.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
        await _socket.SendAsync(new[] { Zero }, SocketFlags.None);
    }

    private async Task<string> ReadAsync()
    {
        byte[] buff = new byte[2_048];
        var sb = new StringBuilder();

        do
        {
            var size = await _socket.ReceiveAsync(buff, SocketFlags.None);
            if (size <= 0) break;
            var part = Encoding.UTF8.GetString(buff, 0, size);
            sb.Append(part);
        } while (buff[^1] != Zero);

        return sb.ToString().Replace("\0", "");
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}