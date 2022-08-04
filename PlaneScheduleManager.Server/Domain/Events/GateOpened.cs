﻿using PlaneScheduleManager.Server.Domain.Events.Interfaces;

namespace PlaneScheduleManager.Server.Domain.Events
{
    public record GateOpened: IDomainEvent
    {
        public static readonly TimeSpan TimeUntilDeparture = TimeSpan.FromHours(1);
        public double MillisecondsUntilEvent { get; init; }
        public string FlightIdentifier { get; init; }
        public string Gate { get; init; }

        public override string ToString()
        {
            return $"The gate {Gate} has been opened for boarding passengers on the flight {FlightIdentifier}";
        }
    }
}