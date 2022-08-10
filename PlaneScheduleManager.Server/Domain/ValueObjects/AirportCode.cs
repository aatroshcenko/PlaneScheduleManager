namespace PlaneScheduleManager.Server.Domain.ValueObjects
{
    public record struct AirportCode
    {
        public string IATACode { get; }

        public AirportCode(
            string code)
        {
            IATACode = code;
        }
    }
}
