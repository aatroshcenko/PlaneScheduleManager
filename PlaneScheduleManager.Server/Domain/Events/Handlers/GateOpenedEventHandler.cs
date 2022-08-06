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
        private readonly ITimerManager _timerManager;

        public GateOpenedEventHandler(
            ITimerManager timerManager,
            IDeviceMessageSender deviceMessageSender,
            IAudioGenerator audioGenerator)
        {
            _deviceMessageSender = deviceMessageSender;
            _audioGenerator = audioGenerator;
            _timerManager = timerManager;

        }
        public void Handle(GateOpened @event)
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
