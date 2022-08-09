using IoTDevice.Client.Models;

namespace IoTDevice.Client.Services.Interfaces
{
    public interface IDevicesClusterManager
    {
        Task HandleAudioMessageAsync(AudioMessage message);

        void HandleDeviceDisconnection(int gateNumber);

        void HandleDeviceConnection(int gateNumber);
        void Lock();

        void Release();
    }
}
