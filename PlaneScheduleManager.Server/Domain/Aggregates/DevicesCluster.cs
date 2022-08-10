using PlaneScheduleManager.Server.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class DevicesCluster
    {
        private readonly ConcurrentDictionary<Gate, Device> _devices = new ConcurrentDictionary<Gate, Device>();

        public Area Area { get; }

        public string GroupName { get => $"Devices_Cluster_{Area}"; }

        public DevicesCluster(
            Area area)
        {
            Area = area;
        }
        
        public void Add(Device device)
        {
            _devices.TryAdd(device.Gate, device);
        }

        public void Remove(Gate gate)
        {
            _devices.TryRemove(gate, out var device);
        }

        public Device Get(Gate gate)
        {
            _devices.TryGetValue(gate, out var device);
            return device;
        }

        public bool IsDeviceConnected(Gate gate)
        {
            return _devices.ContainsKey(gate);
        }
    }
}
