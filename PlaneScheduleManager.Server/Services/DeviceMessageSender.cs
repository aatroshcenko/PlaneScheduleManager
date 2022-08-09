﻿using Microsoft.AspNetCore.SignalR;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Hubs;
using PlaneScheduleManager.Server.Models;
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

        public async Task SendAudioToAllAsync(string audioBase64)
        {
           await _hubContext.Clients.Groups(Device.GroupName)
                .SendAsync("ReceiveAudioMessage",
                new AudioMessage()
                {
                    AudioBase64 = audioBase64
                });
        }

        public async Task SendAudioToAllInAreaAsync(string audioBase64, string area, int gateNumber)
        {
            string areaGroupName = Device.GetAreaGroupName(area);
            await _hubContext.Clients.Groups(areaGroupName)
                .SendAsync("ReceiveAudioMessage",
                new AudioMessage()
                {
                    AudioBase64 = audioBase64,
                    Gate = gateNumber
                });
        }
    }
}
