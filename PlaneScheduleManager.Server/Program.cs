
using Google.Cloud.TextToSpeech.V1;
using PlaneScheduleManager.Server.Domain.Aggregates;
using PlaneScheduleManager.Server.Domain.Events;
using PlaneScheduleManager.Server.Domain.Events.Handlers;
using PlaneScheduleManager.Server.Domain.Events.Interfaces;
using PlaneScheduleManager.Server.Hubs;
using PlaneScheduleManager.Server.Services;
using PlaneScheduleManager.Server.Services.Interfaces;
using PlaneScheduleManager.Server.Utils;
using PlaneScheduleManager.Server.Utils.Interfaces;

var corsPolicyName = "AllowSpecificOrigin";
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(";");
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    });
});
builder.Services.AddSingleton<IDateTimeServer, DateTimeServer>();
builder.Services.AddSingleton<PlaneScheduler>();
builder.Services.AddSingleton<DomainEvents>();
builder.Services.AddSingleton<DomainEventDispatcher>();
builder.Services.AddSingleton<IDevicesManager, DevicesManager>();
builder.Services.AddSingleton<IDomainEventHandler<PlaneArrived>, PlaneArrivedEventHandler>();
builder.Services.AddSingleton<IDomainEventHandler<GateOpened>, GateOpenedEventHandler>();
builder.Services.AddSingleton<IDomainEventHandler<FinalCallMade>, FinalCallEventHandler>();
builder.Services.AddSingleton<TextToSpeechClient>(provider =>
{
    TextToSpeechClientBuilder textToSpeechClientBuilder = new TextToSpeechClientBuilder();
    textToSpeechClientBuilder.CredentialsPath = builder.Configuration["GoogleCredentialsPath"];
    return textToSpeechClientBuilder.Build();
});
builder.Services.AddSingleton<IAudioGenerator, AudioGenerator>();
builder.Services.AddSingleton<IDeviceMessageSender, DeviceMessageSender>();
builder.Services.AddSingleton<ITimerManager, TimerManager>();
builder.Services.AddHostedService<PlaneMovementTrackingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);
app.UseAuthorization();

app.MapControllers();
app.MapHub<DevicesHub>("/devicesHub");

app.Run();
