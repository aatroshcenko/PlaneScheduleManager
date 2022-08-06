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
        private readonly ITimerManager _timerManager;

        public FinalCallEventHandler(
            ITimerManager timerManager,
            IDeviceMessageSender deviceMessageSender,
            IAudioGenerator audioGenerator)
        {
            _deviceMessageSender = deviceMessageSender;
            _audioGenerator = audioGenerator;
            _timerManager = timerManager;
        }
        public void Handle(FinalCallMade @event)
        {
            _timerManager.SetTimeout(async () =>
            {
                ByteString audio = await _audioGenerator.SynthesizeSpeechAsync(@event.ToString());
                await _deviceMessageSender.SendAudioToAllInArea(audio.ToBase64(), @event.Area);
            },
            @event.MillisecondsUntilEvent);
        }
    }
}
