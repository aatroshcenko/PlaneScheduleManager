// See https://aka.ms/new-console-template for more information
using IoTDevice.Client.Domain;
using IoTDevice.Client.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreAudio;
using NetCoreAudio.Interfaces;

Console.WriteLine("Provide the area name");
var area = Console.ReadLine();
var identifier = Guid.NewGuid();
var device = new Device(identifier, area);
var connection = new HubConnectionBuilder()
        .WithUrl($"https://localhost:7157/devicesHub?isManager=false&identifier={identifier}&area={area}")
        .Build();

var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddSingleton<Device>(device);
            services.AddSingleton<HubConnection>(connection);
            services.AddSingleton<IPlayer, Player>();
            services.AddHostedService<DeviceManager>();
        })
        .Build();

await host.RunAsync();