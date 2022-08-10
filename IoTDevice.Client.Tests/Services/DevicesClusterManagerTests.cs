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
        private readonly string AudioBase64 = "NDKLSE";

        private DevicesClusterManager CreateDevicesClusterManager(
            IDeviceManager deviceManager,
            IHubMessageSender hubMessageSender,
            IDateTimeServer dateTimeServer)
        {
            var loggerMock = new Mock<ILogger<DevicesClusterManager>>();
            return new DevicesClusterManager(
                deviceManager,
                hubMessageSender,
                dateTimeServer,
                loggerMock.Object);
        }

        [Fact]
        public async Task Handle_audio_message()
        {
            var deviceManagerMock = new Mock<IDeviceManager>();
            var hubMessageSenderMock = new Mock<IHubMessageSender>();
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(UtcNow);

            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object);

            await sut.HandleAudioMessageAsync(AudioBase64);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(AudioBase64),
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
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object);
            sut.Lock();

            await sut.HandleAudioMessageAsync(AudioBase64);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(AudioBase64),
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
            var sut = CreateDevicesClusterManager(
                deviceManagerMock.Object,
                hubMessageSenderMock.Object,
                dateTimeServerMock.Object);
            sut.Lock();
            Task.Run(() => Task.Delay(1000).ContinueWith((task) => sut.Release()));

            await sut.HandleAudioMessageAsync(AudioBase64);

            hubMessageSenderMock.Verify(x => x.BroadcastClusterLockAsync(),
                Times.Once);
            hubMessageSenderMock.Verify(x => x.BroadcastClusterReleaseAsync(),
                Times.Once);
            deviceManagerMock.Verify(x => x.HandleAudioMessageAsync(AudioBase64),
                Times.Once);
        }

    }
}
