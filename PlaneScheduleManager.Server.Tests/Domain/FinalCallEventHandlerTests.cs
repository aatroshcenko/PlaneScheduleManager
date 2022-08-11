using Google.Protobuf;
using Moq;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Domain.Events.Handlers;
using PlaneScheduleManager.Server.Domain.ValueObjects;
using PlaneScheduleManager.Server.Services;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlaneScheduleManager.Server.Tests.Domain
{
    public class FinalCallEventHandlerTests
    {
        private Gate CreateGate(
            int gateNumber,
            string area)
        {
            return new Gate(gateNumber, new Area(area));
        }

        private FinalCallMade CreateGateOpenedEvent(
            Gate gate)
        {
            return new FinalCallMade(
                    flightId: new FlightId("NW-21"),
                    gate: gate,
                    timeUntilEvent: TimeSpan.Zero);
        }

        private Device CreateDevice(
            Gate gate)
        {
            return new Device(
                "NDLFsfnlanfap",
                new ClientId("123345"),
                gate);
        }

        private FinalCallEventHandler CreateGateOpenedEventHandler(
            IDeviceMessageSender deviceMessageSender,
            Device device)
        {
            var timerManagerMock = new Mock<ITimerManager>();
            timerManagerMock.Setup(x => x.SetTimeout(It.IsAny<Func<Task>>(), It.IsAny<double>()))
                .Returns<Func<Task>, double>((func, time) =>
                {
                    func.Invoke().Wait();
                    return Guid.Empty;
                });
            var devicesManager = new DevicesManager();
            devicesManager.Add(device.ConnectionId, device);
            var audioGeneratorMock = new Mock<IAudioGenerator>();
            audioGeneratorMock.Setup(x => x.SynthesizeSpeechAsync(It.IsAny<string>()))
                .ReturnsAsync(ByteString.FromBase64("NFLKENFW"));
            return new FinalCallEventHandler(
                timerManagerMock.Object,
                deviceMessageSender,
                devicesManager,
                audioGeneratorMock.Object);
        }

        [Fact]
        public void Handle_final_call_event_for_connected_device()
        {
            var deviceMessageSenderMock = new Mock<IDeviceMessageSender>();
            var gate = CreateGate(1, "A");
            var device = CreateDevice(gate);
            var sut = CreateGateOpenedEventHandler(deviceMessageSenderMock.Object, device);
            var @event = CreateGateOpenedEvent(gate);

            sut.Handle(@event);

            deviceMessageSenderMock.Verify(
                x => x.SendAudioToSpecificGateAsync(It.IsAny<string>(), CreateGate(1, "A")),
                Times.Once);

        }

        [Fact]
        public void Handle_final_call_event_for_disconnected_device()
        {
            var deviceMessageSenderMock = new Mock<IDeviceMessageSender>();
            var gate1 = CreateGate(1, "A");
            var gate2 = CreateGate(2, "A");
            var device = CreateDevice(gate1);
            var sut = CreateGateOpenedEventHandler(deviceMessageSenderMock.Object, device);
            var @event = CreateGateOpenedEvent(gate2);

            sut.Handle(@event);

            deviceMessageSenderMock.Verify(
                x => x.SendAudioToAllInAreaAsync(It.IsAny<string>(), new Area("A")),
                Times.Once);
        }


        [Fact]
        public void Failed_to_handle_final_call_event_for_unknown_area()
        {
            var deviceMessageSenderMock = new Mock<IDeviceMessageSender>();
            var gate1 = CreateGate(1, "A");
            var gate2 = CreateGate(2, "B");
            var device = CreateDevice(gate1);
            var sut = CreateGateOpenedEventHandler(deviceMessageSenderMock.Object, device);
            var @event = CreateGateOpenedEvent(gate2);

            Assert.Throws<AggregateException>(() => sut.Handle(@event));

            deviceMessageSenderMock.Verify(
                x => x.SendAudioToAllInAreaAsync(It.IsAny<string>(), It.IsAny<Area>()),
                Times.Never);

            deviceMessageSenderMock.Verify(
                x => x.SendAudioToSpecificGateAsync(It.IsAny<string>(), It.IsAny<Gate>()),
                Times.Never);
        }
    }
}
