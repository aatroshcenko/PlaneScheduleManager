using PlaneScheduleManager.Server.Domain.Events.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public record FinalCallMade: IDomainEvent
    {
        public static readonly TimeSpan TimeUntilDeparture = TimeSpan.FromMinutes(30);
        public double MillisecondsUntilEvent { get; init; }
        public string FlightIdentifier { get; init; }
        public string Area { get; init; }
        public int GateNumber { get; init; }
        public string Gate { get => $"{Area}{GateNumber}"; }

        public override string ToString()
        {
            return $"There are 30 minutes left before the gate {Gate} will have been closed" +
                $" for boarding passengers on the flight {FlightIdentifier}";
        }
    }
}
