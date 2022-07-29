using PlaneScheduleManager.Server.Clients.Interfaces;

namespace PlaneScheduleManager.Server.Clients
{
    public class Device : IClient
    {
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
