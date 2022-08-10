using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Domain.ValueObjects;
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
                _domainEvents.Raise(new FinalCallMade(
                    flightId: new FlightId(planeFlight.FlightIdentifier),
                    gate: new Gate(planeFlight.GateNumber, new Area(planeFlight.Area)),
                    timeUntilEvent: timeUntilDeparture.Add(FinalCallMade.TimeUntilDeparture.Negate()))
                );
            }

            if(timeUntilDeparture >= GateOpened.TimeUntilDeparture)
            {
                _domainEvents.Raise(new GateOpened(
                    flightId: new FlightId(planeFlight.FlightIdentifier),
                    gate: new Gate(planeFlight.GateNumber, new Area(planeFlight.Area)),
                    timeUntilEvent: timeUntilDeparture.Add(GateOpened.TimeUntilDeparture.Negate()))
                );
            }

            if(timeUntilArrival >= TimeSpan.Zero)
            {
                _domainEvents.Raise(new PlaneArrived(
                    flightId: new FlightId(planeFlight.FlightIdentifier),
                    departure: new AirportCode(planeFlight.Departure),
                    destanation: new AirportCode(planeFlight.Destanation),
                    timeUntilEvent: timeUntilArrival
                ));;
            }
        }
    }
}
