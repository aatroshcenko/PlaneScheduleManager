namespace PlaneScheduleManager.Server.Utils.Interfaces
{
    public interface IDateTimeServer
    {
        DateTimeOffset UtcNow { get; }
        long UtcNowTimeStamp { get; }
    }
}
