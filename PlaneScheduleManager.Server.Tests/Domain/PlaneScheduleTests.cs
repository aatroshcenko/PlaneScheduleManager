using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Domain.Events.Handlers;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Services;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlaneScheduleManager.Server.Tests.Domain
{
    public class PlaneScheduleTests
    {
        private readonly DateTimeOffset UtcNow = new DateTimeOffset(2022, 1, 1, 11, 0, 0, TimeSpan.Zero);
        private IServiceProvider CreateServiceProvider(ITimerManager timerManager)
        {
            var devicesManager = new DevicesManager();
            var audioGeneratorMock = new Mock<IAudioGenerator>();
            var deviceMessageSenderMock = new Mock<IDeviceMessageSender>();
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IDevicesManager>(devicesManager);
            serviceCollection.AddSingleton(timerManager);
            serviceCollection.AddSingleton(audioGeneratorMock.Object);
            serviceCollection.AddSingleton(deviceMessageSenderMock.Object);
            serviceCollection.AddSingleton<IDomainEventHandler<PlaneArrived>, PlaneArrivedEventHandler>();
            serviceCollection.AddSingleton<IDomainEventHandler<GateOpened>, GateOpenedEventHandler>();
            serviceCollection.AddSingleton<IDomainEventHandler<FinalCallMade>, FinalCallEventHandler>();

            var serviceProviderFactory = new DefaultServiceProviderFactory();
            var serviceProvider = serviceProviderFactory.CreateServiceProvider(serviceCollection);
            return serviceProvider;
        }

        private PlaneFlight CreatePlaneFlight(
            DateTimeOffset departureTime,
            DateTimeOffset arrivalTime)
        {
            return new PlaneFlight()
            {
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Departure = "A",
                Destanation = "B",
                FlightIdentifier = "N-99"
            };
        }

        private PlaneScheduler CreatePlaneScheduler(DateTimeOffset utcNow, ITimerManager timerManager)
        {
            var domainEvents = new DomainEvents();
            var serviceProvider = CreateServiceProvider(timerManager);
            var domainEventDispatcher = new DomainEventDispatcher(serviceProvider);
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(utcNow);
            return new PlaneScheduler(
                domainEvents,
                domainEventDispatcher,
                dateTimeServerMock.Object);
        }

        [Fact]
        public void Schedule_future_plane_arrival()
        {
            var timerManagerMock = new Mock<ITimerManager>();
            var sut = CreatePlaneScheduler(UtcNow, timerManagerMock.Object);
            var planeFlight = CreatePlaneFlight(UtcNow.AddHours(-1), UtcNow);

            sut.ScheduleFlight(planeFlight);

            timerManagerMock.Verify(
                x => x.SetTimeout(It.IsAny<Func<Task>>(), 0),
                Times.Once);
        }

        [Fact]
        public void Schedule_future_final_call()
        {
            var timerManagerMock = new Mock<ITimerManager>();
            var sut = CreatePlaneScheduler(UtcNow, timerManagerMock.Object);
            var planeFlight = CreatePlaneFlight(
                UtcNow.Add(FinalCallMade.TimeUntilDeparture),
                UtcNow.Add(FinalCallMade.TimeUntilDeparture).AddHours(1));

            sut.ScheduleFlight(planeFlight);

            timerManagerMock.Verify(
                x => x.SetTimeout(It.IsAny<Func<Task>>(), 0),
                Times.Once);
            timerManagerMock.Verify(
                x => x.SetTimeout(
                    It.IsAny<Func<Task>>(), 
                    FinalCallMade.TimeUntilDeparture.Add(TimeSpan.FromHours(1)).TotalMilliseconds),
                Times.Once);
        }

        [Fact]
        public void Schedule_future_gate_opening()
        {
            var timerManagerMock = new Mock<ITimerManager>();
            var sut = CreatePlaneScheduler(UtcNow, timerManagerMock.Object);
            var planeFlight = CreatePlaneFlight(
                UtcNow.Add(GateOpened.TimeUntilDeparture),
                UtcNow.Add(GateOpened.TimeUntilDeparture).AddHours(1));

            sut.ScheduleFlight(planeFlight);

            timerManagerMock.Verify(
                x => x.SetTimeout(It.IsAny<Func<Task>>(), 0),
                Times.Once);
            timerManagerMock.Verify(
                x => x.SetTimeout(
                    It.IsAny<Func<Task>>(),
                    (GateOpened.TimeUntilDeparture - FinalCallMade.TimeUntilDeparture).TotalMilliseconds),
                Times.Once);
            timerManagerMock.Verify(
                x => x.SetTimeout(
                    It.IsAny<Func<Task>>(),
                    GateOpened.TimeUntilDeparture.Add(TimeSpan.FromHours(1)).TotalMilliseconds),
                Times.Once);

        }

        [Fact]
        public void Schedule_no_events_for_past_plane_flight()
        {
            var timerManagerMock = new Mock<ITimerManager>();
            var sut = CreatePlaneScheduler(UtcNow, timerManagerMock.Object);
            var planeFlight = CreatePlaneFlight(UtcNow.AddHours(-1), UtcNow.AddMinutes(-1));

            sut.ScheduleFlight(planeFlight);

            timerManagerMock.Verify(
                x => x.SetTimeout(It.IsAny<Func<Task>>(), It.IsAny<double>()),
                Times.Never);
        }

    }
}
