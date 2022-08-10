using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Services.Interfaces
{
    public interface IDevicesManager
    {
        void Add(string connectionId, Device device);

        bool TryRemove(string connectionId, out Device device);
        Device Get(string connectionId);
        DevicesCluster GetCluster(Area area);
    }
}
