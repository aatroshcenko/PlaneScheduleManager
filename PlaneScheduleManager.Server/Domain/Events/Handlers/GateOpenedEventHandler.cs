using Google.Protobuf;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events.Handlers
{
    public class GateOpenedEventHandler : IDomainEventHandler<GateOpened>
    {
        private readonly IDeviceMessageSender _deviceMessageSender;
        private readonly IAudioGenerator _audioGenerator;
        private readonly IDevicesManager _devicesManager;
        private readonly ITimerManager _timerManager;

        public GateOpenedEventHandler(
            ITimerManager timerManager,
            IDeviceMessageSender deviceMessageSender,
            IDevicesManager deviceManager,
            IAudioGenerator audioGenerator)
        {
            _deviceMessageSender = deviceMessageSender;
            _audioGenerator = audioGenerator;
            _timerManager = timerManager;
            _devicesManager = deviceManager;

        }
        public void Handle(GateOpened @event)
        {
            _timerManager.SetTimeout(async () =>
            {
                var cluster = _devicesManager.GetCluster(@event.Gate.Area);
                if(cluster == null)
                {
                    throw new Exception($"Cluster for area {@event.Gate.Area} was not found.");
                }

                ByteString audio = await _audioGenerator.SynthesizeSpeechAsync(@event.ToString());
                if (cluster.IsDeviceConnected(@event.Gate))
                {
                    await _deviceMessageSender.SendAudioToSpecificGateAsync(audio.ToBase64(), @event.Gate);
                    return;
                }
                await _deviceMessageSender.SendAudioToAllInAreaAsync(audio.ToBase64(), @event.Gate.Area);
            },
            @event.TimeUntilEvent.TotalMilliseconds);
        }
    }
}
