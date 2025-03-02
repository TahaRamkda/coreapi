using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISupervisorService
    {
        Task<string> GetSupervisorDashboardAsync(int clientId, int senderId = 0);
    }
}
