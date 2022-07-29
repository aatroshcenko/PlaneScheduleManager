namespace PlaneScheduleManager.Server.Models
{
    public record DeviceHeartbeat
    {
        public string DeviceId { get; init; }
        public long TimeStamp { get; init; }

        public DeviceStatus Status { get; init; }
    }
}
