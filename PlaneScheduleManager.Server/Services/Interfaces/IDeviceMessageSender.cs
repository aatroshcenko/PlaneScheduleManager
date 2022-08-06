namespace PlaneScheduleManager.Server.Services.Interfaces
{
    public interface IDeviceMessageSender
    {
        Task SendAudioToAll(string audioBase64);

        Task SendAudioToAllInArea(string audioBase64, string area);
    }
}
