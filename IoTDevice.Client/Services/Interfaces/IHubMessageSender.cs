using IoTDevice.Client.Models;

namespace IoTDevice.Client.Services.Interfaces
{
    public interface IHubMessageSender
    {
        Task SendHeartbeatAsync();

        Task SendAudioPlayerStatusAsync(AudioPlayerStatus status);
    }
}
