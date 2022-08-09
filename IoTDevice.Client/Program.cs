// See https://aka.ms/new-console-template for more information
using IoTDevice.Client.Domain;
using IoTDevice.Client.Services;
using IoTDevice.Client.Services.Interfaces;
using IoTDevice.Client.Utils;
using IoTDevice.Client.Utils.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreAudio;
using NetCoreAudio.Interfaces;


var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddSingleton<Device>((provider) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var identifier = configuration["Identifier"];
                var area = configuration["Area"];
                var gate = int.Parse(configuration["Gate"]);
                return new Device(identifier, area, gate);
            });
            services.AddSingleton<HubConnection>((provider) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var device = provider.GetService<Device>();
                var hubUrl = configuration["Urls:Hub"];
                return new HubConnectionBuilder()
                    .WithUrl($"{hubUrl}?isManager=false&identifier={device.Identifier}&area={device.Area}&gate={device.Gate}")
                    .Build();
            });
            services.AddSingleton<IPlayer, Player>();
            services.AddSingleton<IHubMessageSender, HubMessageSender>();
            services.AddSingleton<IAudioManager, AudioManager>();
            services.AddSingleton<IDeviceManager, DeviceManager>();
            services.AddSingleton<IDevicesClusterManager, DevicesClusterManager>();
            services.AddSingleton<IDateTimeServer, DateTimeServer>();
            services.AddHostedService<HubConnectionManager>();
        })
        .Build();

await host.RunAsync();