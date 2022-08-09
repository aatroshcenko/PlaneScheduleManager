using IoTDevice.Client.Domain;
using IoTDevice.Client.Models;
using IoTDevice.Client.Services.Interfaces;
using IoTDevice.Client.Utils.Interfaces;
using Microsoft.Extensions.Logging;

namespace IoTDevice.Client.Services
{
    public class DevicesClusterManager: IDevicesClusterManager
    {
        private static readonly TimeSpan TimeoutPeriod = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan DelayInterval = TimeSpan.FromSeconds(1);
        private readonly Device _device;
        private readonly IDeviceManager _deviceManager;
        private readonly IHubMessageSender _hubMessageSender;
        private readonly IDateTimeServer _dateTimeServer;
        private readonly ILogger<DevicesClusterManager> _logger;
        private volatile bool _isLocked = false;
        
        public DevicesClusterManager(
            Device device,
            IDeviceManager deviceManager,
            IHubMessageSender hubMessageSender,
            IDateTimeServer dateTimeServer,
            ILogger<DevicesClusterManager> logger)
        {
            _device = device;
            _deviceManager = deviceManager;
            _hubMessageSender = hubMessageSender;
            _dateTimeServer = dateTimeServer;
            _logger = logger;
        }

        public async Task HandleAudioMessageAsync(AudioMessage message)
        {
            _logger.LogInformation($"Device {_device.Identifier} received an audio.");
            if (message.Gate.HasValue &&
                _device.Gate != message.Gate && 
                !_device.AdditionalGates.Contains(message.Gate.Value))
            {
                return;
            }
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
            await _deviceManager.HandleAudioMessageAsync(message.AudioBase64);
            await _hubMessageSender.BroadcastClusterReleaseAsync();
        }

        public void HandleDeviceConnection(int gateNumber)
        {
            _device.RemoveGate(gateNumber);
        }

        public void HandleDeviceDisconnection(int gateNumber)
        {
            _device.AddGate(gateNumber);
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
