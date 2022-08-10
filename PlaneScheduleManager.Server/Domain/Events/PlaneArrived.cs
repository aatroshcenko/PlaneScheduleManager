using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public class PlaneArrived: IDomainEvent
    {
        public TimeSpan TimeUntilEvent { get; }
        public FlightId FlightId { get; }
        public AirportCode Departure { get; }
        public AirportCode Destanation { get; }

        public PlaneArrived(
            FlightId flightId,
            AirportCode departure,
            AirportCode destanation,
            TimeSpan timeUntilEvent)
        {
            FlightId = flightId;
            Departure = departure;
            Destanation = destanation;
            TimeUntilEvent = timeUntilEvent;
        }

        public override string ToString()
        {
            return $"The plane {FlightId} has arrived";
        }
    }
}
