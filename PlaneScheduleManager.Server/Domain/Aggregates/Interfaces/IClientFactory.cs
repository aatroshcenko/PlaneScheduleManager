namespace PlaneScheduleManager.Server.Domain.Aggregates.Interfaces
{
    public interface IClientFactory
    {
        IClient Create(string identifier, bool isManager);
    }
}
