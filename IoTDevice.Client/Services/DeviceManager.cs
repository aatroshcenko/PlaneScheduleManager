using IoTDevice.Client.Domain;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreAudio.Interfaces;

namespace IoTDevice.Client.Services
{
    public class DeviceManager : IHostedService, IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly Device _device;
        private readonly IPlayer _player;
        private readonly ILogger<DeviceManager> _logger;
        private Timer _timer;

        private async Task SendHeartbeatAsync()
        {
            await _hubConnection.InvokeAsync("ReceiveDeviceHeartbeat", _device.Identifier);
            _logger.LogInformation("Heartbeat was sent to PlaneScheduleManager.Server.");
        }

        private async Task HandleAudioMessage(string audioBase64)
        {
            _logger.LogInformation("Device got an audio.");
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp3";
            await File.WriteAllBytesAsync(fileName, Convert.FromBase64String(audioBase64));
            await _player.Play(fileName);
        }

        public DeviceManager(
            Device device,
            HubConnection hubConnection,
            IPlayer player,
            ILogger<DeviceManager> logger)
        {
            _device = device;
            _hubConnection = hubConnection;
            _player = player;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {            
            await _hubConnection.StartAsync();
            _logger.LogInformation("Device is started.");

            var heartbeatDelay = TimeSpan.FromSeconds(30);

            _hubConnection.On<string>("RecieveAudioMessage", async (audio) => await HandleAudioMessage(audio));
            _timer = new Timer(async(state) => await SendHeartbeatAsync(), null, TimeSpan.Zero, heartbeatDelay);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Device is stopped."); ;

            _timer?.Change(Timeout.Infinite, 0);
            await _hubConnection.StopAsync();
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
