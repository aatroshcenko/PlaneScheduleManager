using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Domain.Aggregates.Interfaces;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Hubs
{
    public class DevicesHub: Hub
    {
        private readonly ILogger<DevicesHub> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDateTimeServer _dateTimeServer;
        public DevicesHub(
            IMemoryCache memoryCache,
            IDateTimeServer dateTimeServer,
            ILogger<DevicesHub> logger)
        {
            _dateTimeServer = dateTimeServer;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task ReceiveDeviceHeartbeat()
        {
            var client = _memoryCache.Get<IClient>(Context.ConnectionId);
            await Clients.Groups(Manager.GroupName).SendAsync(
                "ReceiveDeviceHeartbeat",
                new DeviceHeartbeat()
                {
                    DeviceId = client.Identifier,
                    TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                    Status = DeviceStatus.Connected
                });
        } 

        public async Task BroadcastClusterLock()
        {
            var client = _memoryCache.Get<Device>(Context.ConnectionId);
            await Clients.Group(client.AreaGroupName).SendAsync(
                "ReceiveClusterLock");

        }

        public async Task BroadcastClusterRelease()
        {
            var client = _memoryCache.Get<Device>(Context.ConnectionId);
            await Clients.Group(client.AreaGroupName).SendAsync(
                "ReceiveClusterRelease");

        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            _logger.LogInformation($"Client with connectionId '{Context.ConnectionId}' was connected.");

            var httpContext = Context.GetHttpContext();
            if(!bool.TryParse(httpContext.Request.Query["isManager"], out var isManager))
            {
                throw new BadHttpRequestException("'isManager' query parameter is invalid.");
            }

            var identifier = httpContext.Request.Query["identifier"];

            if (isManager)
            {
                _memoryCache.Set<IClient>(Context.ConnectionId, new Manager(identifier));
                await Groups.AddToGroupAsync(Context.ConnectionId, Manager.GroupName);               
            }
            else
            {
                var area = httpContext.Request.Query["area"];
                var gate =int.Parse(httpContext.Request.Query["gate"]);
                var device = new Device(identifier, area, gate);
                _memoryCache.Set<IClient>(Context.ConnectionId, device);
                await Groups.AddToGroupAsync(Context.ConnectionId, Device.GroupName);
                
                await Clients.Groups(Manager.GroupName)
                    .SendAsync("ReceiveDeviceHeartbeat", new DeviceHeartbeat()
                    {
                        DeviceId = identifier,
                        TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                        Status = DeviceStatus.Connected
                    });
                await Clients.Groups(device.AreaGroupName).SendAsync("ReceiveConnectedDevice", device.Gate);
                await Groups.AddToGroupAsync(Context.ConnectionId, device.AreaGroupName);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            var client = _memoryCache.Get<IClient>(Context.ConnectionId);
            if(client != null && !client.IsManager())
            {
                var device = (Device)client;
                await Clients.Groups(Manager.GroupName)
                    .SendAsync("ReceiveDeviceHeartbeat", new DeviceHeartbeat()
                    {
                        DeviceId = client.Identifier,
                        TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                        Status = DeviceStatus.Disconnected
                    });
                await Clients.Groups(device.AreaGroupName)
                    .SendAsync("ReceiveDisconnectedDevice", device.Gate);

            }
            _logger.LogInformation($"Client with connectionId '{Context.ConnectionId}' was disconnected.");
        }
    }
}
