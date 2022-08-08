using IoTDevice.Client.Models;
using IoTDevice.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace IoTDevice.Client.Services
{
    internal class HubMessageSender: IHubMessageSender
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<HubMessageSender> _logger;

        public HubMessageSender(
            HubConnection hubConnection,
            ILogger<HubMessageSender> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
        }
        public async Task SendHeartbeatAsync()
        {
            await _hubConnection.InvokeAsync("ReceiveDeviceHeartbeat");
            _logger.LogInformation("Heartbeat was sent to PlaneScheduleManager.");
        }

        public async Task SendAudioPlayerStatusAsync(AudioPlayerStatus status)
        {
            await _hubConnection.InvokeAsync("BroadcastAudioPlayerStatus", status);
            _logger.LogInformation("Player status '{status}' was sent to PlaneScheduleManager.", status.ToString());
        }
    }
}
