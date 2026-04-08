using Microsoft.EntityFrameworkCore;
using Serilog;
using SportsStore.Models;
using SportsStore.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "SportsStore-Blazor")
    .WriteTo.Console()
    .WriteTo.File("logs/sportsstore-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("OrderApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OrderApi:BaseUrl"] ?? "http://localhost:5000");
});

builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    opts.UseSqlite(builder.Configuration["ConnectionStrings:SportsStoreConnection"]
        ?? "Data Source=SportsStore.db");
});

builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorComponents<SportsStore.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();

SeedData.EnsurePopulated(app);

Log.Information("SportsStore Blazor application starting...");

app.Run();
