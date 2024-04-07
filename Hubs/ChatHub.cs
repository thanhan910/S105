using Microsoft.AspNetCore.SignalR;

namespace S105.Hubs;

public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHttpClientFactory _clientFactory;

    public ChatHub(IHubContext<ChatHub> hubContext, IHttpClientFactory clientFactory)
    {
        _hubContext = hubContext;
        _clientFactory = clientFactory;
    }

    public async Task NewMessage(long username, string message) =>
        await Clients.All.SendAsync("messageReceived", username, message);

    public async override Task OnConnectedAsync()
    {
        // No need for a timer here, SignalR manages connections
        await base.OnConnectedAsync();
        await FetchAndSendData();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    private async Task FetchAndSendData()
    {
        using var httpClient = _clientFactory.CreateClient();
        var response = await httpClient.GetAsync("https://timetableapi.ptv.vic.gov.au/swagger/docs/v3"); // Replace with your target URL
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        Console.WriteLine(data);
        await _hubContext.Clients.All.SendAsync("dataReceived", data);
    }
}