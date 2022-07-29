using PlaneScheduleManager.Server.Clients.Interfaces;

namespace PlaneScheduleManager.Server.Clients
{
    public class ClientFactory : IClientFactory
    {
        public IClient Create(string identifier, bool isManager)
        {
            return isManager ? new Manager(identifier) : new Device(identifier);
        }
    }
}
