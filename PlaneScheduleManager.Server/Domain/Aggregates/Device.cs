using PlaneScheduleManager.Server.Domain.Aggregates.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Aggregates
{
    public class Device : IClient
    {
        public static readonly string GroupName = "Devices";
        public string AreaGroupName { get => GetAreaGroupName(Area); }
        public string Identifier { get; }

        public string Area { get; }

        public int Gate { get; }


        public static string GetAreaGroupName(string area)
        {
            return $"{GroupName}_Area_{area}";
        }

        public Device(string identifier, string area, int gate)
        {
            Identifier = identifier;
            Area = area;
            Gate = gate;
        }

        public bool IsManager()
        {
            return false;
        }
    }
}
