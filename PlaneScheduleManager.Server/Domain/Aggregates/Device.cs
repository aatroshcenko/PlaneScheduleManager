using PlaneScheduleManager.Server.Domain.Aggregates.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class Device : IClient
    {
        public static readonly string GroupName = "Devices";
        public string Identifier { get; }

        public bool IsManager()
        {
            return false;
        }
        public Device(string identifier)
        {
            Identifier = identifier;
        }
    }
}
