using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Clients.Interfaces;
using PlaneScheduleManager.Server.Clients;
using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Hubs
{
    public class DevicesHub: Hub
    {
        private readonly ILogger<DevicesHub> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IClientFactory _clientFactory;
        private readonly IDateTimeServer _dateTimeServer;
        public DevicesHub(
            IMemoryCache memoryCache,
            IClientFactory clientFactory,
            IDateTimeServer dateTimeServer,
            ILogger<DevicesHub> logger)
        {
            _dateTimeServer = dateTimeServer;
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task ReceiveDeviceHeartbeat(string identifier)
        {
            await Clients.Groups(Manager.GroupName).SendAsync(
                "ReceiveDeviceHeartbeat",
                new DeviceHeartbeat()
                {
                    DeviceId = identifier,
                    TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                    Status = DeviceStatus.Connected
                });
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
            IClient client = _clientFactory.Create(identifier, isManager);
            _memoryCache.Set<IClient>(Context.ConnectionId, client);

            if (isManager)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Manager.GroupName);
            }
            else
            {
                await Clients.Groups(Manager.GroupName)
                    .SendAsync("ReceiveDeviceHeartbeat", new DeviceHeartbeat()
                    {
                        DeviceId = identifier,
                        TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                        Status = DeviceStatus.Connected
                    });
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            var client = _memoryCache.Get<IClient>(Context.ConnectionId);
            if(client != null && !client.IsManager())
            {
                await Clients.Groups(Manager.GroupName)
                    .SendAsync("ReceiveDeviceHeartbeat", new DeviceHeartbeat()
                    {
                        DeviceId = client.Identifier,
                        TimeStamp = _dateTimeServer.UtcNowTimeStamp,
                        Status = DeviceStatus.Disconnected
                    });
                _memoryCache.Remove(Context.ConnectionId);
            }
            _logger.LogInformation($"Client with connectionId '{Context.ConnectionId}' was disconnected.");
        }
    }
}
