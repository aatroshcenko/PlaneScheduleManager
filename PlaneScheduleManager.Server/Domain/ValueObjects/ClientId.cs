namespace PlaneScheduleManager.Server.Domain.ValueObjects
{
    public record struct ClientId
    {
        public string Value { get; }

        public ClientId(
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
