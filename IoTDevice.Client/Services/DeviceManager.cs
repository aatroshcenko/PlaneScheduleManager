using IoTDevice.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace IoTDevice.Client.Services
{
    public class DeviceManager : IDeviceManager
    {
        private readonly HubConnection _hubConnection;
        private readonly IAudioManager _audioManager;
        private readonly ILogger<DeviceManager> _logger;
        

        public DeviceManager(
            HubConnection hubConnection,
            IAudioManager audioManager,
            ILogger<DeviceManager> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
            _audioManager = audioManager;
        }


        public async Task SendHeartbeatAsync()
        {
            await _hubConnection.InvokeAsync("ReceiveDeviceHeartbeat");
            _logger.LogInformation("Heartbeat was sent to PlaneScheduleManager.Server.");
        }

        public async Task HandleAudioMessageAsync(string audioBase64)
        {
            string fileName = await _audioManager.CreateTempMp3FileAsync(audioBase64);
            await _audioManager.PlayAudioAsync(fileName);
        }
    }
}
