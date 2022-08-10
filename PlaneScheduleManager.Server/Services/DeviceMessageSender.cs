using Microsoft.AspNetCore.SignalR;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.ValueObjects;
using PlaneScheduleManager.Server.Hubs;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Services.Interfaces;

namespace PlaneScheduleManager.Server.Services
{
    public class DeviceMessageSender: IDeviceMessageSender
    {
        private readonly IDevicesManager _deviceManager;
        private readonly IHubContext<DevicesHub> _hubContext;
        public DeviceMessageSender(
            IDevicesManager devicesManager,
            IHubContext<DevicesHub> hubContext)
        {
            _deviceManager = devicesManager;
            _hubContext = hubContext;
        }

        public async Task SendAudioToAllAsync(string audioBase64)
        {
           await _hubContext.Clients.Groups(Device.GroupName)
                .SendAsync("ReceiveAudioMessage", audioBase64);
        }

        public async Task SendAudioToAllInAreaAsync(string audioBase64, Area area)
        {
            var cluster = _deviceManager.GetCluster(area);
            await _hubContext.Clients.Groups(cluster.GroupName)
                .SendAsync("ReceiveAudioMessage", audioBase64);
        }

        public async Task SendAudioToSpecificGateAsync(string audioBase64, Gate gate)
        {
            var cluster = _deviceManager.GetCluster(gate.Area);
            var device = cluster.Get(gate);
            await _hubContext.Clients.Client(device.ConnectionId)
                .SendAsync("ReceiveAudioMessage", audioBase64);
        }
    }
}
