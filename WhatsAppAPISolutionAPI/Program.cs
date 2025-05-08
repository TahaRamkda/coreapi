using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;
using WhatsAppAPISolutionAPI.Extensions;
using WhatsAppAPISolutionAPI.Helper;
using WhatsAppAPISolutionAPI.Middleware;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Hubs;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//Add support to logging with SERILOG
//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());

//Logging related logic
var loggingEnabled = builder.Configuration.GetValue<bool>("LogSettings:LoggingEnabled");
if (loggingEnabled)
{
    // Get log level from configuration
    var queueSize = builder.Configuration.GetValue<long>("Axiom:QueueLimitBytes");
    var logLevel = builder.Configuration.GetValue<string>("LogSettings:LogLevel");
    var minLevel = logLevel switch
    {
        "Debug" => LogEventLevel.Debug,
        "Information" => LogEventLevel.Information,
        "Warning" => LogEventLevel.Warning,
        "Error" => LogEventLevel.Error,
        _ => LogEventLevel.Information // Default level
    };

    var logger = new LoggerConfiguration()
                 .MinimumLevel.Is(minLevel) // Dynamically apply level
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)  // Suppress low-level framework logs
                 .MinimumLevel.Override("System", LogEventLevel.Error)  // Only show Errors for System logs
                 .WriteTo.Http(
                     requestUri: builder.Configuration["Axiom:LogURL"],
                     queueLimitBytes: queueSize,
                     httpClient: new CustomHttpClient(),
                     configuration: builder.Configuration)
                 .CreateLogger();

    builder.Host.UseSerilog(logger);
}

// Core framework services
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerServices();
builder.Services.AddHttpClient<ConversationAnalytics>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

// Add services to the container.
builder.Services.AddApplicationServices(configuration);
builder.Services.AddDatabaseServices(configuration);
builder.Services.AddSettingServices(configuration);
builder.Services.AddHttpClientServices(configuration);
builder.Services.AddOneSignalServices(configuration);
builder.Services.AddFlowEndpointServices(configuration);
builder.Services.AddAuthorizationServices(configuration);
builder.Services.AddSignalRServices(configuration);
//builder.Services.
// MVC Controllers
builder.Services.AddControllers();

//builder.Services.AddHttpClient<KFGPaymentService>();

var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Media/")),
    RequestPath = new PathString("/Media")
});

bool enableGlobalExceptionHandler = Convert.ToBoolean(builder.Configuration["EnableGlobalExceptionHandler"]);
if (enableGlobalExceptionHandler)
    app.UseExceptionHandlerMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger((c =>
    {
        c.RouteTemplate = "/WhatsAppAPISolutiondocs/swagger/{documentName}/swagger.json";
    }));
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/WhatsAppAPISolutiondocs/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "WhatsAppAPISolutiondocs"; // Custom route
    });
}

app.UseHttpsRedirection();
// Serve static files (place early to avoid unnecessary pipeline processing)
app.UseStaticFiles();
// Enable CORS before auth if you need it for preflight requests
app.UseCors();
// Add authentication before authorization
app.UseAuthentication();
app.UseAuthorization();
// Map controllers
app.MapControllers();
// Map SignalR hubs
app.MapHub<ConversationHub>("/Conversation");
// Start the app
app.Run();
