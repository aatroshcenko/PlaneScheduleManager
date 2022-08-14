using CsvHelper;
using CsvHelper.Configuration;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Mappers;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Utils.Interfaces;
using System.Globalization;
using System.Linq;

namespace PlaneScheduleManager.Server.Services
{
    public class PlaneMovementTrackingService : IHostedService
    {
        private int previousRowsCount = 0;
        private readonly ILogger<PlaneMovementTrackingService> _logger;
        private readonly ITimerManager _timerManager;
        private readonly PlaneScheduler _planeScheduler;
        private readonly string _flightDatasetPath;
        private Guid _intervalId;


        public PlaneMovementTrackingService(
            IConfiguration configuration,
            ITimerManager timerManager,
            ILogger<PlaneMovementTrackingService> logger,
            PlaneScheduler planeScheduler)
        {
            _logger = logger;
            _planeScheduler = planeScheduler;
            _timerManager = timerManager;
            _flightDatasetPath = configuration["DatasetPath"];
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Plane movement tracking service is running.");

            _intervalId = _timerManager.SetInterval(TrackChanges, TimeSpan.FromMinutes(1).TotalMilliseconds);

            return Task.CompletedTask;
        }

        private void TrackChanges()
        {
            if (!File.Exists(_flightDatasetPath))
            {
                _logger.LogWarning($"Dataset file was not found at '{Path.GetFullPath(_flightDatasetPath)}'");
                return;
            }
            var currentRowsCount = previousRowsCount;
            using(FileStream fs = File.Open(_flightDatasetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

            _timerManager.ClearInterval(_intervalId);

            return Task.CompletedTask;
        }
    }
}
