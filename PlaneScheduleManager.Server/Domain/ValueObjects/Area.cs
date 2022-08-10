namespace PlaneScheduleManager.Server.Domain.ValueObjects
{
    public record struct Area
    {
        public string Name { get; }

        public Area(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
