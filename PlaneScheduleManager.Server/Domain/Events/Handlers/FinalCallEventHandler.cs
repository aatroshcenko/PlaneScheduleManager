using Google.Protobuf;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events.Handlers
{
    public class FinalCallEventHandler : IDomainEventHandler<FinalCallMade>
    {
        private readonly IDeviceMessageSender _deviceMessageSender;
        private readonly IAudioGenerator _audioGenerator;
        private readonly IDevicesManager _devicesManager;
        private readonly ITimerManager _timerManager;

        public FinalCallEventHandler(
            ITimerManager timerManager,
            IDeviceMessageSender deviceMessageSender,
            IDevicesManager deviceManager,
            IAudioGenerator audioGenerator)
        {
            _deviceMessageSender = deviceMessageSender;
            _audioGenerator = audioGenerator;
            _devicesManager = deviceManager;
            _timerManager = timerManager;
        }
        public void Handle(FinalCallMade @event)
        {

            _timerManager.SetTimeout(async () =>
            {
                ByteString audio = await _audioGenerator.SynthesizeSpeechAsync(@event.ToString());
                var cluster = _devicesManager.GetCluster(@event.Gate.Area);
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
