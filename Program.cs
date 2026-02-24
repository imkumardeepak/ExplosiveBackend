using Microsoft.AspNetCore.Authorization;
using Peso_Baseed_Barcode_Printing_System_API.Configuration;
using Peso_Baseed_Barcode_Printing_System_API.Extensions;
using Peso_Baseed_Barcode_Printing_System_API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/app_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add all application services
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Global Exception Handler
app.UseGlobalExceptionHandler();

// Unhandled Exception Handlers
AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var exception = args.ExceptionObject as Exception;
    logger.LogCritical(exception, "Unhandled domain exception occurred. IsTerminating: {IsTerminating}", args.IsTerminating);
};

TaskScheduler.UnobservedTaskException += (sender, args) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(args.Exception, "Unobserved task exception occurred");
    args.SetObserved();
};

// Configure WebSockets
app.ConfigureWebSockets(builder.Configuration);
app.ConfigureNotificationEndpoint();

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Barcode Printing API V1");
    });
}

// CORS
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Security
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapGet("/", () => "Welcome to the ASP.NET Core Barcode Printing System API!");

app.Run();
