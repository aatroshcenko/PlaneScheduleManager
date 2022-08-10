using IoTDevice.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IoTDevice.Client.Services
{
    public class HubConnectionManager: IHostedService, IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly IDeviceManager _deviceManager;
        private readonly IDevicesClusterManager _devicesClusterManager;
        private readonly ILogger<HubConnectionManager> _logger;
        private Timer _timer;

        public HubConnectionManager(
            HubConnection hubConnection,
            IDeviceManager deviceManager,
            IDevicesClusterManager devicesClusterManager,
            ILogger<HubConnectionManager> logger)
        {
            _hubConnection = hubConnection;
            _deviceManager = deviceManager;
            _devicesClusterManager = devicesClusterManager;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _hubConnection.StartAsync();
            _logger.LogInformation("Device is started.");

            var heartbeatDelay = TimeSpan.FromSeconds(30);

            _hubConnection.On<string>("ReceiveAudioMessage", 
                async (audio) => await _devicesClusterManager.HandleAudioMessageAsync(audio));
            _hubConnection.On("ReceiveClusterLock",
                () => _devicesClusterManager.Lock());
            _hubConnection.On("ReceiveClusterRelease",
                () => _devicesClusterManager.Release());
            _timer = new Timer(async (state) => await _deviceManager.SendHeartbeatAsync(), null, TimeSpan.Zero, heartbeatDelay);
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
