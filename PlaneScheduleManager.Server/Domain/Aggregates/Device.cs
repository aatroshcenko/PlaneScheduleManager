using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class Device
    {
        public static readonly string GroupName = "Devices";

        public string ConnectionId { get; }

        public ClientId Id { get; }

        public Gate Gate { get; }

        public Device(
            string connectionId,
            ClientId id,
            Gate gate)
        {
            ConnectionId = connectionId;
            Id = id;
            Gate = gate;
        }
    }
}
