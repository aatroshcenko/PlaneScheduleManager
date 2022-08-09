namespace PlaneScheduleManager.Server.Services.Interfaces
{
    public interface IDeviceMessageSender
    {
        Task SendAudioToAllAsync(string audioBase64);

        Task SendAudioToAllInAreaAsync(string audioBase64, string area, int gateNumber);
    }
}
