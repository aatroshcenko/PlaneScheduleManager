using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Utils.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class PlaneScheduler
    {
        private readonly DomainEvents _domainEvents;
        private readonly IDateTimeServer _dateTimeServer;
        public PlaneScheduler(
            DomainEvents domainEvents,
            DomainEventDispatcher domainEventDispatcher,
            IDateTimeServer dateTimeServer)
        {
            _domainEvents = domainEvents;
            _dateTimeServer = dateTimeServer;
            domainEvents.Register<PlaneArrived>(ev => domainEventDispatcher.Dispatch(ev));
            domainEvents.Register<GateOpened>(ev => domainEventDispatcher.Dispatch(ev));
            domainEvents.Register<FinalCallMade>(ev => domainEventDispatcher.Dispatch(ev));
        }

        public void ScheduleFlight(PlaneFlight planeFlight)
        {
            var utcNow = _dateTimeServer.UtcNow;
            var timeUntilDeparture = planeFlight.DepartureTime - utcNow;
            var timeUntilArrival = planeFlight.ArrivalTime - utcNow;
            if (timeUntilDeparture >= FinalCallMade.TimeUntilDeparture)
            {
                _domainEvents.Raise(new FinalCallMade()
                {
                    MillisecondsUntilEvent = timeUntilDeparture.Add(FinalCallMade.TimeUntilDeparture.Negate()).TotalMilliseconds,
                    FlightIdentifier = planeFlight.FlightIdentifier,
                    GateNumber = planeFlight.GateNumber,
                    Area = planeFlight.Area
                });
            }

            if(timeUntilDeparture >= GateOpened.TimeUntilDeparture)
            {
                _domainEvents.Raise(new GateOpened()
                {
                    MillisecondsUntilEvent = timeUntilDeparture.Add(GateOpened.TimeUntilDeparture.Negate()).TotalMilliseconds,
                    FlightIdentifier = planeFlight.FlightIdentifier,
                    GateNumber = planeFlight.GateNumber,
                    Area = planeFlight.Area
                });
            }

            if(timeUntilArrival >= TimeSpan.Zero)
            {
                _domainEvents.Raise(new PlaneArrived()
                {
                    MillisecondsUntilEvent = timeUntilArrival.TotalMilliseconds,
                    FlightIdentifier = planeFlight.FlightIdentifier,
                    Departure = planeFlight.Departure,
                    Destanation = planeFlight.Destanation
                });;
            }
        }
    }
}
