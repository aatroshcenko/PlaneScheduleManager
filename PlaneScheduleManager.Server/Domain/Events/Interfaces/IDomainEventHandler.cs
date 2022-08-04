namespace PlaneScheduleManager.Server.Domain.Events.Interfaces
{
    public interface IDomainEventHandler<in T>
        where T : IDomainEvent
    {
        void Handle(T @event);
    }
}
