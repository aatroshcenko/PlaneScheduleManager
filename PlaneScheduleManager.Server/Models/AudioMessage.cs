namespace PlaneScheduleManager.Server.Models
{
    public record AudioMessage
    {
        public int? Gate { get; init; }
        public string AudioBase64 { get; init; }
    }
}
