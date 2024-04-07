using Microsoft.AspNetCore.SignalR;

namespace S105.Hubs;

public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHttpClientFactory _clientFactory;
    private Timer _timer;

    public ChatHub(IHubContext<ChatHub> hubContext, IHttpClientFactory clientFactory)
    {
        _hubContext = hubContext;
        _clientFactory = clientFactory;

        // Start the timer when the hub is constructed
        _timer = new Timer(async _ => await FetchAndSendData(), null, TimeSpan.Zero, TimeSpan.FromMicroseconds(500));
    }

    public async Task NewMessage(long username, string message) =>
        await Clients.All.SendAsync("messageReceived", username, message);

    public override Task OnConnectedAsync()
    {
        // No need for a timer here, SignalR manages connections
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Dispose the timer when the hub is disconnected
        _timer.Dispose();
        return base.OnDisconnectedAsync(exception);
    }

    private async Task FetchAndSendData()
    {
        while (true) // Continue fetching data indefinitely
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://timetableapi.ptv.vic.gov.au/swagger/docs/v3");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            await _hubContext.Clients.All.SendAsync("dataReceived", data);

            // Wait for a certain interval before fetching data again
            await Task.Delay(TimeSpan.FromMicroseconds(200)); // Example: wait for 10 seconds before fetching again
        }
    }
}
