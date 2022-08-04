using PlaneScheduleManager.Server.Domain.Events.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public record PlaneArrived: IDomainEvent
    {
        public double MillisecondsUntilEvent { get; init; }
        public string FlightIdentifier { get; init; }
        public string Departure { get; init; }
        public string Destanation { get; init; }

        public override string ToString()
        {
            return $"The plane {FlightIdentifier} has arrived";
        }
    }
}
