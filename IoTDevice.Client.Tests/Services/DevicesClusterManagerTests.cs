using IoTDevice.Client.Domain;
using IoTDevice.Client.Models;
using IoTDevice.Client.Services;
using IoTDevice.Client.Services.Interfaces;
using IoTDevice.Client.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IoTDevice.Client.Tests.Services
{
    public class DevicesClusterManagerTests
    {
        private readonly DateTimeOffset UtcNow = new DateTimeOffset(2022, 1, 1, 11, 0, 0, TimeSpan.Zero);
        private Device CreateDevice(
            string identifer = "1234",
            string area = "A",
            int gate = 1)
        {
            return new Device(identifer, area, gate);
        }

        private DevicesClusterManager CreateDevicesClusterManager(
            IDeviceManager deviceManager,
            IHubMessageSender hubMessageSender,
            IDateTimeServer dateTimeServer,
            Device device)
        {
            var loggerMock = new Mock<ILogger<DevicesClusterManager>>();
            return new DevicesClusterManager(
                device,
                deviceManager,
                hubMessageSender,
                dateTimeServer,
                loggerMock.Object);
        }

        private AudioMessage CreateAudioMessage(int? gate = null)
        {
            return new AudioMessage()
            {
                AudioBase64 = "NDSLKE",
                Gate = gate
            };
        }

        [Fact]
        public async Task Handle_plane_arrival_event_audio_message()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(UtcNow);

            var device = CreateDevice();
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object,
                device);
            var audioMessage = CreateAudioMessage();

            await sut.HandleAudioMessageAsync(audioMessage);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(audioMessage.AudioBase64),
                Times.Once);
        }

        [Fact]
        public async Task Handle_plane_departure_event_audio_message_for_an_other_gate()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(UtcNow);
            var gate = 1;
            var device = CreateDevice(gate: gate);
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object,
                device);
            var audioMessage = CreateAudioMessage(gate: gate + 1);

            await sut.HandleAudioMessageAsync(audioMessage);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Never);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Never);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(audioMessage.AudioBase64),
                Times.Never);
        }

        [Fact]
        public async Task Handle_plane_departure_event_audio_message_for_the_same_gate()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(UtcNow);
            var gate = 1;
            var device = CreateDevice(gate: gate);
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object,
                device);
            var audioMessage = CreateAudioMessage(gate: gate);

            await sut.HandleAudioMessageAsync(audioMessage);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(audioMessage.AudioBase64),
                Times.Once);
        }

        [Fact]
        public async Task Handle_plane_departure_event_audio_message_for_an_other_disconnected_device()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(UtcNow);
            var gate = 1;
            var device = CreateDevice(gate: gate);
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object,
                device);
            var audioMessage = CreateAudioMessage(gate: gate + 1);
            sut.HandleDeviceDisconnection(gate + 1);

            await sut.HandleAudioMessageAsync(audioMessage);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(audioMessage.AudioBase64),
                Times.Once);
        }

        [Fact]
        public async Task Handle_concurrent_audio_messages_by_timeout()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            var currenUtcNow = UtcNow;
            var timeout = TimeSpan.FromMinutes(5);
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(() =>
                {
                    var prevUtcNow = currenUtcNow;
                    currenUtcNow = currenUtcNow.Add(timeout);
                    return prevUtcNow;
                });
            var device = CreateDevice();
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object,
                device);
            var audioMessage = CreateAudioMessage();
            sut.Lock();

            await sut.HandleAudioMessageAsync(audioMessage);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(audioMessage.AudioBase64),
                Times.Once);
        }

        [Fact]
        public async Task Handle_concurrent_audio_messages()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(UtcNow);
            var device = CreateDevice();
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object,
                device);
            var audioMessage = CreateAudioMessage();
            sut.Lock();
            Task.Run(() => Task.Delay(1000).ContinueWith((task) => sut.Release()));

            await sut.HandleAudioMessageAsync(audioMessage);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(audioMessage.AudioBase64),
                Times.Once);
        }

    }
}
