using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.ValueObjects;
using PlaneScheduleManager.Server.Services.Interfaces;
using System.Collections.Concurrent;

namespace PlaneScheduleManager.Server.Services
{
    public class DevicesManager: IDevicesManager
    {
        private readonly ConcurrentDictionary<string, Device> _devices = new ConcurrentDictionary<string, Device>();
        private readonly ConcurrentDictionary<Area, DevicesCluster> _clusters = new ConcurrentDictionary<Area, DevicesCluster>();

        public void Add(string connectionId, Device device)
        {
            if(_devices.TryAdd(connectionId, device))
            {
                var cluster = _clusters.GetOrAdd(device.Gate.Area, new DevicesCluster(device.Gate.Area));
                cluster.Add(device);
            };
        }

        public Device Get(string connectionId)
        {
            _devices.TryGetValue(connectionId, out var device);
            return device;
        }

        public DevicesCluster GetCluster(Area area)
        {
            _clusters.TryGetValue(area, out var cluster);
            return cluster;
        }

        public bool TryRemove(string connectionId, out Device device)
        {
            var isRemoved = _devices.TryRemove(connectionId, out device);
            if (!isRemoved)
            {
                return false;
            };
            var cluster = GetCluster(device.Gate.Area);
            cluster.Remove(device.Gate);
            return true;
        }
    }
}
