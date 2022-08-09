using IoTDevice.Client.Utils.Interfaces;

namespace IoTDevice.Client.Utils
{
    public class DateTimeServer: IDateTimeServer
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
