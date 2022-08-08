namespace IoTDevice.Client.Services.Interfaces
{
    public interface IAudioManager
    {
        Task<string> CreateTempMp3FileAsync(string audioBase64);
        Task PlayAudioAsync(string fileName);
    }
}
