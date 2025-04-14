using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using WhatsAppAPISolutionAPI.Middleware;
using WhatsAppAPISolutionDL.Setting;
using System.Text;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services, IConfiguration config)
        {
            // Adding Authentication
            services.AddAuthentication(options =>
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
                    ValidAudience = config["Jwt:Audience"],
                    ValidIssuer = config["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthenticationSchemes.ApiKeyPolicy, policy => policy.RequireAuthenticatedUser().AddAuthenticationSchemes("ApiKey"));
                options.AddPolicy(AuthenticationSchemes.BearerPolicy, policy => policy.RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
            });

            return services;
        }
    }
}
