using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Services.Interfaces
{
    public interface IDeviceMessageSender
    {
        Task SendAudioToAllAsync(string audioBase64);

        Task SendAudioToAllInAreaAsync(string audioBase64, Area area);

        Task SendAudioToSpecificGateAsync(string audioBase64, Gate gate);
    }
}
