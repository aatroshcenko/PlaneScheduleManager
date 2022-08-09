using IoTDevice.Client.Models;

namespace IoTDevice.Client.Services.Interfaces
{
    public interface IDeviceManager
    {
        Task SendHeartbeatAsync();
        Task HandleAudioMessageAsync(string audioBase64);
    }
}
