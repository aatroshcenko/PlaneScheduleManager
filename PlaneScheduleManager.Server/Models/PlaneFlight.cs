namespace PlaneScheduleManager.Server.Models
{
    public record PlaneFlight
    {
        public DateTimeOffset DepartureTime { get; init; }
        public DateTimeOffset ArrivalTime { get; init; }
        public string FlightIdentifier { get; init; }
        public string Departure { get; init; }
        public string Destanation { get; init; }
        public string Gate { get; init; }
    }
}
