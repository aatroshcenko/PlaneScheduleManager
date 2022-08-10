using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public class GateOpened: IDomainEvent
    {
        public static readonly TimeSpan TimeUntilDeparture = TimeSpan.FromHours(1);
        public TimeSpan TimeUntilEvent { get; }
        public FlightId FlightId{ get; }
        public Gate Gate { get; }

        public GateOpened(
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
            return $"The gate {Gate} has been opened for boarding passengers on the flight {FlightId}";
        }
    }
}
