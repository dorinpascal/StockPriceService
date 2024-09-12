using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace SimpleTestClient;

public static class SignalRClientTesting
{
    private const string url = "http://localhost:7012/api/negotiate";
    public static async Task StartClient()
    {
        Console.WriteLine("Starting SignalR client...");

        var connection = await InitializeSignalRConnectionAsync(url);

        if (connection is null)
        {
            Console.WriteLine("Failed to establish SignalR connection.");
            return;
        }

        SetUpSignalRHandlers(connection);

        await StartSignalRConnectionAsync(connection);
    }


    // Method to initialize the SignalR connection
    private static async Task<HubConnection?> InitializeSignalRConnectionAsync(string url)
    {
        var response = await GetSignalRConnectionInfoAsync(url);

        if (response is null)
        {
            Console.WriteLine("Failed to retrieve SignalR connection info.");
            return null;
        }

        // Build the SignalR connection
        return new HubConnectionBuilder()
            .WithUrl(response.Url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(response.AccessToken)!;
            })
            .Build();
    }

    // Method to set up event handlers
    private static void SetUpSignalRHandlers(HubConnection connection)
    {
        connection.On<object[]>("stockPriceUpdated", (stockPrices) =>
        {
            Console.WriteLine("Received stock price updates:");

            foreach (var stockPrice in stockPrices)
            {
                Console.WriteLine(stockPrice.ToString());
            }
        });
    }

    // Method to start and manage the SignalR connection
    private static async Task StartSignalRConnectionAsync(HubConnection connection)
    {
        try
        {
            await connection.StartAsync();
            Console.WriteLine("SignalR connection established.");

            // Keep the console open
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Stop the connection when finished
            await connection.StopAsync();
            Console.WriteLine("SignalR connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to SignalR hub: {ex.Message}");
        }
    }


    // Method to call the Negotiate function and retrieve connection info (URL and token)
    private static async Task<NegotiateResponse?> GetSignalRConnectionInfoAsync(string negotiateUrl)
    {
        using var client = new HttpClient();
        try
        {
            // Call the negotiate function
            var response = await client.PostAsync(negotiateUrl, null);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to negotiate SignalR connection: {response.StatusCode}");
                return null;
            }

            // Parse the response
            var content = await response.Content.ReadAsStringAsync();
            var negotiateResponse = JsonSerializer.Deserialize<NegotiateResponse>(content);

            // Return the URL and access token from the negotiate response
            return negotiateResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during negotiation: {ex.Message}");
            return null;
        }
    }
}
