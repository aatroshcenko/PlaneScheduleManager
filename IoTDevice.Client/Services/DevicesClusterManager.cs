using IoTDevice.Client.Services.Interfaces;
using IoTDevice.Client.Utils.Interfaces;
using Microsoft.Extensions.Logging;

namespace IoTDevice.Client.Services
{
    public class DevicesClusterManager: IDevicesClusterManager
    {
        private static readonly TimeSpan TimeoutPeriod = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan DelayInterval = TimeSpan.FromSeconds(1);
        private readonly IDeviceManager _deviceManager;
        private readonly IHubMessageSender _hubMessageSender;
        private readonly IDateTimeServer _dateTimeServer;
        private readonly ILogger<DevicesClusterManager> _logger;
        private volatile bool _isLocked = false;
        
        public DevicesClusterManager(
            IDeviceManager deviceManager,
            IHubMessageSender hubMessageSender,
            IDateTimeServer dateTimeServer,
            ILogger<DevicesClusterManager> logger)
        {
            _deviceManager = deviceManager;
            _hubMessageSender = hubMessageSender;
            _dateTimeServer = dateTimeServer;
            _logger = logger;
        }

        public async Task HandleAudioMessageAsync(string audioBase64)
        {
            _logger.LogInformation($"Device received an audio.");

            var recievTime = _dateTimeServer.UtcNow;

            while (_isLocked)
            {
                _logger.LogInformation("Other device in cluster is doing work. Waiting...");
                if(_dateTimeServer.UtcNow - recievTime > TimeoutPeriod)
                {
                    _isLocked = false;
                    break;
                }
                await Task.Delay(DelayInterval);
            }

            await _hubMessageSender.BroadcastClusterLockAsync();
            await _deviceManager.HandleAudioMessageAsync(audioBase64);
            await _hubMessageSender.BroadcastClusterReleaseAsync();
        }
        public void Lock()
        {
            _isLocked = true;
        }

        public void Release()
        {
            _isLocked = false;
        }
    }
}
