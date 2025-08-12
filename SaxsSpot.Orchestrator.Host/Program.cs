using SaxsSpot.Orchestrator.Application.Extensions;
using SaxsSpot.Orchestrator.Infrastructure.Extensions;
using SaxsSpot.Orchestrator.Kafka;
using SaxsSpot.Orchestrator.Kafka.Extensions;
using SaxsSpot.Shared.Authenticator.Extensions;
using SaxsSpot.Shared.ProgressTrackerClient.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsSettings>();
builder.Services.Configure<KafkaConfiguration>(builder.Configuration.GetSection("kafka"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins)
            .WithMethods(corsSettings.AllowedMethods)
            .WithHeaders(corsSettings.AllowedHeaders);
        
        if (corsSettings.AllowCredentials)
        {
            policy.AllowCredentials();
        }
        else
        {
            policy.DisallowCredentials();
        }
    });
});

builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddEnvironmentVariables();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddCalculationServiceStorage();
builder.Services.AddJobServiceClient(builder.Configuration);
builder.Services.AddAuthenticator(builder.Configuration);
var conf = builder.Configuration;

builder.Services.AddOpenApi();
builder.Services.AddKafkaEventing(conf);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("MyCorsPolicy");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; }
    public string[] AllowedMethods { get; set; }
    public string[] AllowedHeaders { get; set; }
    public bool AllowCredentials { get; set; }
}