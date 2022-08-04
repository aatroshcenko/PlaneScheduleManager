// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using NetCoreAudio;
using NetCoreAudio.Interfaces;

var player = new Player();
var identifier = Guid.NewGuid();
var connection = new HubConnectionBuilder()
        .WithUrl($"https://localhost:7157/devicesHub?isManager=false&identifier={identifier}")
        .Build();

connection.On<string>("RecieveAudioMessage", async (audio) => await HandleAudioMessage(player, audio));
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

async Task HandleAudioMessage(IPlayer player, string audioBase64)
{
    string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp3";
    await File.WriteAllBytesAsync(fileName, Convert.FromBase64String(audioBase64));
    await player.Play(fileName);
}