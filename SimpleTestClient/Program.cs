using SimpleTestClient;

namespace TestSignalR;

internal static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing real time communication...");
        await SignalRClientTesting.StartClient();
    }
}