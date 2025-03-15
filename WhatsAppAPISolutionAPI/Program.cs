using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WhatsAppAPISolutionAPI.Extensions;
using WhatsAppAPISolutionAPI.Helper;
using WhatsAppAPISolutionAPI.Middleware;
using WhatsAppAPISolutionDL.Hubs;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//Add support to logging with SERILOG
//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());

// Get log level from configuration
var logLevel = builder.Configuration.GetValue<string>("Logging:LogLevel:Default");
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
                 queueLimitBytes: null,
                 httpClient: new CustomHttpClient(),
                 configuration: builder.Configuration)
             .CreateLogger();

builder.Host.UseSerilog(logger);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices(configuration);
builder.Services.AddDatabaseServices(configuration);
builder.Services.AddSettingServices(configuration);
builder.Services.AddHttpClientServices(configuration);
builder.Services.AddOneSignalServices(configuration);
builder.Services.AddFlowEndpointServices(configuration);

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = configuration["Jwt:Audience"],
        ValidIssuer = configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllers();

// Configure SignalR options
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(Convert.ToInt32(configuration["SignalRConfiguration:KeepAliveInterval"]));  //
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(Convert.ToInt32(configuration["SignalRConfiguration:ClientTimeoutInterval"]));  //
    options.HandshakeTimeout = TimeSpan.FromSeconds(Convert.ToInt32(configuration["SignalRConfiguration:HandshakeTimeout"])); //
    options.EnableDetailedErrors = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(setup =>
{
    var schemaHelper = new SwashbuckleSchemaHelper();
    setup.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));

    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

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
app.UseCors();
app.UseAuthentication();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ConversationHub>("/Conversation");
app.Run();
