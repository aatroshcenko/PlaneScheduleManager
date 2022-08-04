namespace PlaneScheduleManager.Server.Services.Interfaces
{
    public interface IDeviceMessageSender
    {
        Task SendAudioToAll(string audioBase64);
    }
}
