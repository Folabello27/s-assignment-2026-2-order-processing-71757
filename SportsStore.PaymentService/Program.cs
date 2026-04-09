using Serilog;
using SportsStore.Infrastructure.Messaging;
using SportsStore.PaymentService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "PaymentService")
    .WriteTo.Console()
    .WriteTo.File("logs/payment-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add RabbitMQ
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var rabbitMqPort = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672");
var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

var rabbitMqConnection = new RabbitMqConnection(rabbitMqHost, rabbitMqPort, rabbitMqUser, rabbitMqPass);
builder.Services.AddSingleton(rabbitMqConnection);
builder.Services.AddHostedService<PaymentProcessingConsumer>();

var app = builder.Build();

Log.Information("Payment Service starting...");

app.Run();
