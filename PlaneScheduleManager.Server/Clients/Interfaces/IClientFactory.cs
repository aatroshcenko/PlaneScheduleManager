namespace PlaneScheduleManager.Server.Clients.Interfaces
{
    public interface IClientFactory
    {
        IClient Create(string identifier, bool isManager);
    }
}
