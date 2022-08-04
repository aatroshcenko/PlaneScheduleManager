namespace PlaneScheduleManager.Server.Domain.Aggregates.Interfaces
{
    public interface IClient
    {
        string Identifier { get; }
        bool IsManager();
    }
}
