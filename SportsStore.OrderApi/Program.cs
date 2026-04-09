using Microsoft.EntityFrameworkCore;
using Serilog;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Mapping;
using SportsStore.Infrastructure.Messaging;
using SportsStore.OrderApi.Consumers;
using SportsStore.OrderApi.Data;
using SportsStore.OrderApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "OrderApi")
    .WriteTo.Console()
    .WriteTo.File("logs/order-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<SportsStore.Core.CQRS.Commands.CheckoutOrderCommand>();
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Add DbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=orders.db");
});

// Add repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();

// Add RabbitMQ
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var rabbitMqPort = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672");
var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

var rabbitMqConnection = new RabbitMqConnection(rabbitMqHost, rabbitMqPort, rabbitMqUser, rabbitMqPass);
builder.Services.AddSingleton(rabbitMqConnection);
builder.Services.AddSingleton<IEventPublisher>(sp =>
    new RabbitMqPublisher(sp.GetRequiredService<RabbitMqConnection>()));
builder.Services.AddHostedService<InventoryResultConsumer>();
builder.Services.AddHostedService<InventoryFailedConsumer>();
builder.Services.AddHostedService<PaymentResultConsumer>();
builder.Services.AddHostedService<PaymentRejectedConsumer>();
builder.Services.AddHostedService<ShippingResultConsumer>();
builder.Services.AddHostedService<OrderCompletedConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
