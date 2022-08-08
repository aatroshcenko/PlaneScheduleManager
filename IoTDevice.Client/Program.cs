// See https://aka.ms/new-console-template for more information
using IoTDevice.Client.Models;
using IoTDevice.Client.Services;
using IoTDevice.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreAudio;
using NetCoreAudio.Interfaces;


var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddSingleton<HubConnection>((provider) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var hubUrl = configuration["Urls:Hub"];
                var identifier = configuration["Identifier"];
                var area = configuration["Area"];
                return new HubConnectionBuilder()
                    .WithUrl($"{hubUrl}?isManager=false&identifier={identifier}&area={area}")
                    .Build();
            });
            services.AddSingleton<IPlayer, Player>();
            services.AddSingleton<IHubMessageSender, HubMessageSender>();
            services.AddSingleton<IAudioManager, AudioManager>();
            services.AddHostedService<DeviceManager>();
        })
        .Build();

await host.RunAsync();