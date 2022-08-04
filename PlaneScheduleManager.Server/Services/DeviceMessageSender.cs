using Microsoft.AspNetCore.SignalR;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Hubs;
using PlaneScheduleManager.Server.Services.Interfaces;

namespace PlaneScheduleManager.Server.Services
{
    public class DeviceMessageSender: IDeviceMessageSender
    {
        private readonly IHubContext<DevicesHub> _hubContext;
        public DeviceMessageSender(
            IHubContext<DevicesHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAudioToAll(string audioBase64)
        {
           await _hubContext.Clients.Groups(Device.GroupName)
                .SendAsync("RecieveAudioMessage",
                audioBase64);
        }
    }
}
