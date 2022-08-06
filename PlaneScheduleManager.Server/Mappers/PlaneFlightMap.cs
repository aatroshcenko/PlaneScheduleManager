using CsvHelper.Configuration;
using PlaneScheduleManager.Server.Models;
using System.Globalization;

namespace PlaneScheduleManager.Server.Mappers
{
    public class PlaneFlightMap: ClassMap<PlaneFlight>
    {
        public PlaneFlightMap()
        {
            Map(m => m.DepartureTime).Convert(x => DateTimeOffset.Parse(
                x.Row.GetField("DepTime"),
                DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.AssumeUniversal));
            Map(m => m.ArrivalTime).Convert(x => DateTimeOffset.Parse(
                x.Row.GetField("ArrTime"),
                DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.AssumeUniversal));
            Map(m => m.FlightIdentifier).Convert(x => $"{x.Row.GetField("UniqueCarrier")}-{x.Row.GetField("FlightNum")}");
            Map(m => m.Departure).Name("Dep");
            Map(m => m.Destanation).Name("Dest");
            Map(m => m.Area);
            Map(m => m.GateNumber).Name("GateNum");
        }
    }
}
