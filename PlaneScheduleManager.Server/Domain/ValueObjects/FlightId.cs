namespace PlaneScheduleManager.Server.Domain.ValueObjects
{
    public record struct FlightId
    {
        public string Value { get; }

        public FlightId(
            string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
