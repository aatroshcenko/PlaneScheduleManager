using PlaneScheduleManager.Server.Domain.Events.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public class DomainEvents
    {
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public void Register<T>(Action<T> eventHandler)
            where T : IDomainEvent
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
            {
                _handlers.Add(type, eventHandler);
                return;
            }          
            _handlers[type] = Delegate.Combine(_handlers[type], eventHandler);
        }

        public void Raise<T>(T domainEvent)
            where T : IDomainEvent
        {
            var handler = _handlers[domainEvent.GetType()] as Action<T>;
            handler?.Invoke(domainEvent);
        }
    }
}
