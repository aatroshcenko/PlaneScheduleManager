using CsvHelper;
using CsvHelper.Configuration;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Mappers;
using PlaneScheduleManager.Server.Models;
using System.Globalization;
using System.Linq;

namespace PlaneScheduleManager.Server.Services
{
    public class PlaneMovementTrackingService : IHostedService, IDisposable
    {
        private int previousRowsCount = 0;
        private readonly ILogger<PlaneMovementTrackingService> _logger;
        private Timer? _timer = null;
        private readonly PlaneScheduler _planeScheduler;


        public PlaneMovementTrackingService(
            ILogger<PlaneMovementTrackingService> logger,
            PlaneScheduler planeScheduler)
        {
            _logger = logger;
            _planeScheduler = planeScheduler;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Plane movement tracking service is running.");

            _timer = new Timer(TrackChanges, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));


            return Task.CompletedTask;
        }

        private void TrackChanges(object? state)
        {
            const string filePath = @"./Datasets/AirlineDataset/Flights/Flights.csv";
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Dataset file was not found at '{Path.GetFullPath(filePath)}'");
                return;
            }
            var currentRowsCount = previousRowsCount;
            using(FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            using(CsvReader csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine
            }))
            {
                csvReader.Context.RegisterClassMap<PlaneFlightMap>();
                var records = new List<PlaneFlight>();
                csvReader.Read();
                csvReader.ReadHeader();
                while (csvReader.Read())
                {
                    var record = csvReader.GetRecord<PlaneFlight>();
                    records.Add(record);
                }
                currentRowsCount = records.Count;
                foreach (var record in records.Skip(previousRowsCount))
                {
                    _planeScheduler.ScheduleFlight(record);
                }
            }
            Interlocked.CompareExchange(ref previousRowsCount, currentRowsCount, previousRowsCount);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Plane movement tracking service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
