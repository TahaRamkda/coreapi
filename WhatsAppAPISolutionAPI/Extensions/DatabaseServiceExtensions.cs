using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class DatabaseServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration config)
        {
            int commandTimeout = 0;
            int.TryParse(config["CommandTimeout"], out commandTimeout);

            services.AddDbContext<WhatsAppSolutionContext>(options => options.UseSqlServer(config.GetConnectionString("WhatsAppAPISolutionDataBase"), sqlOptions => sqlOptions.CommandTimeout(commandTimeout)));
            services.AddDbContext<WhatsAppSolutionContext2>(options => options.UseSqlServer(config.GetConnectionString("WhatsAppAPISolutionDataBase"), sqlOptions => sqlOptions.CommandTimeout(commandTimeout)));

            return services;
        }
    }
}
