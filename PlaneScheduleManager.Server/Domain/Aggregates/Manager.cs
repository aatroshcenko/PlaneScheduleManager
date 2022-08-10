using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class Manager
    {
        public static readonly string GroupName = "Manager";
        public ClientId Id { get; }
        public Manager(ClientId id)
        {
            Id = id;
        }
    }
}
