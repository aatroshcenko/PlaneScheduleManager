using IoTDevice.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IoTDevice.Client.Services
{
    public class DeviceManager : IHostedService, IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly IAudioManager _audioManager;
        private readonly ILogger<DeviceManager> _logger;
        private Timer _timer;

        private async Task SendHeartbeatAsync()
        {
            await _hubConnection.InvokeAsync("ReceiveDeviceHeartbeat");
            _logger.LogInformation("Heartbeat was sent to PlaneScheduleManager.Server.");
        }

        private async Task HandleAudioMessage(string audioBase64)
        {
            _logger.LogInformation("Device got an audio.");
            string fileName = await _audioManager.CreateTempMp3FileAsync(audioBase64);
            await _audioManager.PlayAudioAsync(fileName);
        }

        public DeviceManager(
            HubConnection hubConnection,
            IAudioManager audioManager,
            ILogger<DeviceManager> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
            _audioManager = audioManager;
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
