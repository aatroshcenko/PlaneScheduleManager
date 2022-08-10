using PlaneScheduleManager.Server.Domain.ValueObjects;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public record struct Gate
    {
        public int Number { get; }

        public Area Area { get; }

        public Gate(
            int number,
            Area area)
        {
            Number = number;
            Area = area;
        }

        public override string ToString()
        {
            return $"{Area}{Number}";
        }
    }
}
