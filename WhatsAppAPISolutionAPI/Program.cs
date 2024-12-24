using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using WhatsAppAPISolutionAPI.Helper;
using WhatsAppAPISolutionAPI.Middleware;
using WhatsAppAPISolutionAPI.Security;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());

//var allowedOrigins = configuration["Origins"];
//var allowedOriginLists = allowedOrigins.Split(';');
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(builder =>
//        {
//            builder.WithOrigins("*").WithHeaders("*")
//                   .AllowAnyMethod();
//        });
//});

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IExportManager, ExportManager>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ISenderNameService, SenderNameService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ICustomIntegrationService, CustomIntegrationService>();
builder.Services.AddScoped<IAPIMessageService, APIMessageService>();
builder.Services.AddScoped<IMessageSentLogsService, MessageSentLogsService>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IAgentsService, AgentsService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IImportManager, ImportManager>();

builder.Services.AddDbContext<WhatsAppSolutionContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WhatsAppAPISolutionDataBase")));
builder.Services.AddDbContext<WhatsAppSolutionContext2>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WhatsAppAPISolutionDataBase")));

builder.Services.Configure<BridgeConfigurationSettings>(builder.Configuration.GetSection(BridgeConfigurationSettings.ConfigKey));
builder.Services.Configure<APISolutionConfigurationSettings>(builder.Configuration.GetSection(APISolutionConfigurationSettings.ConfigKey));

builder.Services.AddHttpClient(HttpClientType.bridge_api, (serviceProvider, httpClient) =>
{
    var whatsAppConfiguration = serviceProvider.GetRequiredService<IOptions<BridgeConfigurationSettings>>().Value;

    httpClient.BaseAddress = new Uri(whatsAppConfiguration.BaseURL);
    httpClient.Timeout = TimeSpan.FromSeconds(whatsAppConfiguration.TimeOutInSeconds);
});

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
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
builder.Services.AddSignalR();

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
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads/")),

    RequestPath = new PathString("/Uploads")
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
