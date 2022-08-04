using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Domain.Events.Handlers;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Models;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlaneScheduleManager.Server.Tests.Domain
{
    public class PlaneScheduleTests
    {
        private IServiceProvider CreateServiceProvider(ITimerManager timerManager)
        {            
            var audioGeneratorMock = new Mock<IAudioGenerator>();
            var deviceMessageSenderMock = new Mock<IDeviceMessageSender>();
            var serviceCollection = new ServiceCollection();

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

        [Fact]
        public void Schedule_future_plane_arrival()
        {
            var utcNow = new DateTimeOffset(2022, 1, 1, 11, 0, 0, TimeSpan.Zero);
            var domainEvents = new DomainEvents();
            var timerManagerMock = new Mock<ITimerManager>();
            var serviceProvider = CreateServiceProvider(timerManagerMock.Object);
            var domainEventDispatcher = new DomainEventDispatcher(serviceProvider);
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(utcNow);
            var planeFlight = CreatePlaneFlight(utcNow.AddHours(-1), utcNow);
            var sut = new PlaneScheduler(
                domainEvents,
                domainEventDispatcher,
                dateTimeServerMock.Object);

            sut.ScheduleFlight(planeFlight);

            timerManagerMock.Verify(
                x => x.SetTimeout(It.IsAny<Func<Task>>(), 0),
                Times.Once);
        }

        [Fact]
        public void Schedule_future_final_call()
        {
            var utcNow = new DateTimeOffset(2022, 1, 1, 11, 0, 0, TimeSpan.Zero);
            var domainEvents = new DomainEvents();
            var timerManagerMock = new Mock<ITimerManager>();
            var serviceProvider = CreateServiceProvider(timerManagerMock.Object);
            var domainEventDispatcher = new DomainEventDispatcher(serviceProvider);
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(utcNow);
            var planeFlight = CreatePlaneFlight(
                utcNow.Add(FinalCallMade.TimeUntilDeparture),
                utcNow.Add(FinalCallMade.TimeUntilDeparture).AddHours(1));
            var sut = new PlaneScheduler(
                domainEvents,
                domainEventDispatcher,
                dateTimeServerMock.Object);

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
            var utcNow = new DateTimeOffset(2022, 1, 1, 11, 0, 0, TimeSpan.Zero);
            var domainEvents = new DomainEvents();
            var timerManagerMock = new Mock<ITimerManager>();
            var serviceProvider = CreateServiceProvider(timerManagerMock.Object);
            var domainEventDispatcher = new DomainEventDispatcher(serviceProvider);
            var dateTimeServerMock = new Mock<IDateTimeServer>();
            dateTimeServerMock.Setup(x => x.UtcNow)
                .Returns(utcNow);
            var planeFlight = CreatePlaneFlight(
                utcNow.Add(GateOpened.TimeUntilDeparture),
                utcNow.Add(GateOpened.TimeUntilDeparture).AddHours(1));
            var sut = new PlaneScheduler(
                domainEvents,
                domainEventDispatcher,
                dateTimeServerMock.Object);

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
    }
}
