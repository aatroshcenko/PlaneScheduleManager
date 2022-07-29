using PlaneScheduleManager.Server.Clients.Interfaces;

namespace PlaneScheduleManager.Server.Clients
{
    public class Manager : IClient
    {
        public static readonly string GroupName = "Manager";
        public string Identifier { get; }
        public bool IsManager()
        {
            return true;
        }

        public Manager(string identifier)
        {
            Identifier = identifier;
        }
    }
}
