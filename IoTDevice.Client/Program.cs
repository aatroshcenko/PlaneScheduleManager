// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;

var identifier = Guid.NewGuid();
var connection = new HubConnectionBuilder()
        .WithUrl($"https://localhost:7157/devicesHub?isManager=false&identifier={identifier}")
        .Build();

await connection.StartAsync();

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var worker = Task.Run(async () =>
{
    var delay = TimeSpan.FromSeconds(30);
    while (!cancellationToken.IsCancellationRequested)
    {
        await connection.InvokeAsync("ReceiveDeviceHeartbeat", identifier);
        Console.WriteLine("Heartbeat was sent to PlaneScheduleManager.Server.");
        await Task.Delay(delay);
    }
}, cancellationToken).ContinueWith((t) =>
{
    if (t.IsFaulted)
    {
        Console.WriteLine("Something went wrong.");
        Console.WriteLine(t.Exception.Message);
    }
});

Console.WriteLine("Enter any key to stop.");
Console.ReadLine();
cancellationTokenSource.Cancel();
await connection.StopAsync();
