namespace IoTDevice.Client.Services.Interfaces
{
    public interface IDevicesClusterManager
    {
        Task HandleAudioMessageAsync(string audioBase64);
        void Lock();
        void Release();
    }
}
