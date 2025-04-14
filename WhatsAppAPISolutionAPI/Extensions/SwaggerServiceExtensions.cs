using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using WhatsAppAPISolutionAPI.Helper;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            //services.AddSwaggerGen(setup =>
            //{
            //    var schemaHelper = new SwashbuckleSchemaHelper();
            //    setup.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));

            //    // Include 'SecurityScheme' to use JWT Authentication
            //    var jwtSecurityScheme = new OpenApiSecurityScheme
            //    {
            //        BearerFormat = "JWT",
            //        Name = "JWT Authentication",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.Http,
            //        Scheme = JwtBearerDefaults.AuthenticationScheme,
            //        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

            //        Reference = new OpenApiReference
            //        {
            //            Id = JwtBearerDefaults.AuthenticationScheme,
            //            Type = ReferenceType.SecurityScheme
            //        }
            //    };

            //    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            //    setup.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });

            //});

            services.AddSwaggerGen(setup =>
            { 
                var schemaHelper = new SwashbuckleSchemaHelper();
                setup.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter the Bearer token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                setup.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter the API key",
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}
