
using PlaneScheduleManager.Server.Clients;
using PlaneScheduleManager.Server.Clients.Interfaces;
using PlaneScheduleManager.Server.Hubs;
using PlaneScheduleManager.Server.Utils;
using PlaneScheduleManager.Server.Utils.Interfaces;

var corsPolicyName = "AllowSpecificOrigin";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("https//localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    });
});
builder.Services.AddSingleton<IClientFactory, ClientFactory>();
builder.Services.AddSingleton<IDateTimeServer, DateTimeServer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthorization();

app.MapControllers();
app.MapHub<DevicesHub>("/devicesHub");

app.Run();
