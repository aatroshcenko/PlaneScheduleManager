using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public sealed class DomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public DomainEventDispatcher(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch(IDomainEvent domainEvent)
        {
            Type type = typeof(IDomainEventHandler<>);
            Type[] typeArgs = { domainEvent.GetType() };
            Type handlerType = type.MakeGenericType(typeArgs);

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.Handle((dynamic)domainEvent);
        }
    }
}
