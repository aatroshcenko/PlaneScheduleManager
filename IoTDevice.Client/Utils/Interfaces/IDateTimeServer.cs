namespace IoTDevice.Client.Utils.Interfaces
{
    public interface IDateTimeServer
    {
        DateTimeOffset UtcNow { get; }
    }
}
