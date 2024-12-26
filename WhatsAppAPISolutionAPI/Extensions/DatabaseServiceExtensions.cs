using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class DatabaseServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<WhatsAppSolutionContext>(options => options.UseSqlServer(config.GetConnectionString("WhatsAppAPISolutionDataBase")));
            services.AddDbContext<WhatsAppSolutionContext2>(options => options.UseSqlServer(config.GetConnectionString("WhatsAppAPISolutionDataBase")));

            return services;
        }
    }
}
