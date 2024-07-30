using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.BlazorClient.Components.Pages;

public partial class Home : IAsyncDisposable
{
    private HubConnection? hubConnection;
    private List<string> messages = [];
    private string? username;
    private string? message;
    private bool isConnected => hubConnection?.State == HubConnectionState.Connected;

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7159/chatHub")
        .Build();

        hubConnection.On<string, string>("ReceiveMessage", (username, message) =>
        {
            var encodedMessage = $"{username}: {message}";
            messages.Add(encodedMessage);
            StateHasChanged();
        });

        await hubConnection.StartAsync();
    }

    private async Task SendMessage()
    {
        if (hubConnection != null && isConnected)
        {
            await hubConnection!.InvokeAsync("SendMessage", username, message);
            message = string.Empty;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
