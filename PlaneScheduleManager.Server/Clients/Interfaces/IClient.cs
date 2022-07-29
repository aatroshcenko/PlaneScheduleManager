namespace PlaneScheduleManager.Server.Clients.Interfaces
{
    public interface IClient
    {
        string Identifier { get; }
        bool IsManager();
    }
}
