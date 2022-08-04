using PlaneScheduleManager.Server.Domain.Aggregates.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class ClientFactory : IClientFactory
    {
        public IClient Create(string identifier, bool isManager)
        {
            return isManager ? new Manager(identifier) : new Device(identifier);
        }
    }
}
