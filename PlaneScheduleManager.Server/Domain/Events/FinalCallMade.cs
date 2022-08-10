using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public class FinalCallMade: IDomainEvent
    {
        public static readonly TimeSpan TimeUntilDeparture = TimeSpan.FromMinutes(30);
        public TimeSpan TimeUntilEvent { get; }
        public FlightId FlightId { get;}
        public Gate Gate { get; }

        public FinalCallMade(
            FlightId flightId,
            Gate gate,
            TimeSpan timeUntilEvent)
        {
            FlightId = flightId;
            Gate = gate;
            TimeUntilEvent = timeUntilEvent;
        }

        public override string ToString()
        {
            return $"There are 30 minutes left before the gate {Gate} will have been closed" +
                $" for boarding passengers on the flight {FlightId}";
        }
    }
}
