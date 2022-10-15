using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SpeechSearchSystem.Consumers.Freeling;

internal class FreelingService : IDisposable
{
    private const string MessageResetStats = "RESET_STATS";
    private const string MessageFlushBuffer = "FLUSH_BUFFER";
    private const string MessageServerReady = "FL-SERVER-READY";
    private const byte Zero = (byte)0;
    private const int BufferSize = 2_048;

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
        _socket.ReceiveTimeout = -1;
        _socket.SendTimeout = -1;
        _socket.ReceiveBufferSize = BufferSize;
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
    }

    public async Task<string> ProcessMorphologicalAnalysisAsync(string text)
    {
        Console.WriteLine("\tConnect to freeling");
        await _socket.ConnectAsync(_ipEndPoint);

        Console.WriteLine("\tMessageResetStats");
        await WriteAsync(MessageResetStats);
        Console.WriteLine("\tReady?");
        var readyTask = ReadAsync();
        var ready = await readyTask.WaitAsync(TimeSpan.FromSeconds(15));
        if (ready != MessageServerReady)
        {
            throw new Exception("Server not ready");
        }

        Console.WriteLine(MessageFlushBuffer);
        await WriteAsync(MessageFlushBuffer);
        await ReadAsync();

        await WriteAsync(text);
        var response = await ReadAsync();
        Console.WriteLine($"\n\tRESPONSE:\n\t{response}\n\tEND_RESPONSE\n");

        Console.WriteLine(MessageFlushBuffer);
        await WriteAsync(MessageFlushBuffer);
        await ReadAsync();
        Console.WriteLine();
        return response;
    }

    private async Task WriteAsync(string message)
    {
        await _socket.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
        await _socket.SendAsync(new[] { Zero }, SocketFlags.None);
    }

    private async Task<string> ReadAsync()
    {
        byte[] buff = new byte[BufferSize];
        var sb = new StringBuilder();

        int size;
        do
        {
            size = await _socket.ReceiveAsync(buff, SocketFlags.None);
            if (size <= 0) break;
            var part = Encoding.UTF8.GetString(buff, 0, size);
            sb.Append(part);
        } while (buff[size - 1] != Zero);

        return sb.ToString().Replace("\0", "");
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}