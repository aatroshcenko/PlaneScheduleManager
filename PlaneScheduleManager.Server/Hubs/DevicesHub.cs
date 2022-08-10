using Microsoft.AspNetCore.SignalR;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.ValueObjects;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Hubs
{
    public class DevicesHub: Hub
    {
        private readonly ILogger<DevicesHub> _logger;
        private readonly IDateTimeServer _dateTimeServer;
        private readonly IDevicesManager _devicesManager;
        public DevicesHub(
            IDateTimeServer dateTimeServer,
            IDevicesManager devicesManager,
            ILogger<DevicesHub> logger)
        {
            _dateTimeServer = dateTimeServer;
            _devicesManager = devicesManager;
            _logger = logger;
        }

        public async Task ReceiveDeviceHeartbeat()
        {
            var device = _devicesManager.Get(Context.ConnectionId);
            await Clients.Groups(Manager.GroupName).SendAsync(
                "ReceiveDeviceHeartbeat",
                new DeviceHeartbeat()
                {
                    DeviceId = device.Id.Value,
                    TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                    Status = DeviceStatus.Connected
                });
        } 

        public async Task BroadcastClusterLock()
        {
            var device = _devicesManager.Get(Context.ConnectionId);
            var cluster = _devicesManager.GetCluster(device.Gate.Area);
            await Clients.Group(cluster.GroupName).SendAsync(
                "ReceiveClusterLock");

        }

        public async Task BroadcastClusterRelease()
        {
            var device = _devicesManager.Get(Context.ConnectionId);
            var cluster = _devicesManager.GetCluster(device.Gate.Area);
            await Clients.Group(cluster.GroupName).SendAsync(
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

            var clientId = httpContext.Request.Query["clientId"];

            if (isManager)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Manager.GroupName);
                return;
            }

            var area = httpContext.Request.Query["area"];
            var gate = int.Parse(httpContext.Request.Query["gate"]);
            var device = new Device(
                     connectionId: Context.ConnectionId,
                     id: new ClientId(clientId),
                     gate: new Gate(gate, new Area(area))
                );
            await Groups.AddToGroupAsync(Context.ConnectionId, Device.GroupName);
            _devicesManager.Add(Context.ConnectionId, device);
            var cluster = _devicesManager.GetCluster(device.Gate.Area);
            await Groups.AddToGroupAsync(Context.ConnectionId, cluster.GroupName);
            await Clients.Groups(Manager.GroupName)
                .SendAsync("ReceiveDeviceHeartbeat", new DeviceHeartbeat()
                {
                    DeviceId = clientId,
                    TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                    Status = DeviceStatus.Connected
                });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"Client with connectionId '{Context.ConnectionId}' was disconnected.");
            if(!_devicesManager.TryRemove(Context.ConnectionId, out var device))
            {
                return;
            }
            
            await Clients.Groups(Manager.GroupName)
                .SendAsync("ReceiveDeviceHeartbeat", new DeviceHeartbeat()
                {
                    DeviceId = device.Id.Value,
                    TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                    Status = DeviceStatus.Disconnected
                });
        }
    }
}
