using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class SupervisorService: ISupervisorService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public SupervisorService(WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }

        public async Task<string> GetSupervisorDashboardAsync(int clientId, int senderId = 0)
        {
            var response = await _dbContext2.JsonDatas.FromSqlInterpolated($"exec usp_Supervisor_Dashboard @ClientId={clientId}, @SenderId={senderId}").ToListAsync();
            if (response != null && response.Any())
                return response[0].JsonDataStr;

            return String.Empty;
        }
    }
}
