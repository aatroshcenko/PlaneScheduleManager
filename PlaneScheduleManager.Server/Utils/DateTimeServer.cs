using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Utils
{
    public class DateTimeServer : IDateTimeServer
    {
        public long UtcNowTimeStamp => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
