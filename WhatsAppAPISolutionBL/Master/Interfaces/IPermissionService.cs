using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IPermissionService
    {
        public Task<List<UPermissionList>> GetPermissionListAsync(int client_Id, int role_Id = 0);
        public Task<UResponse> AddPermissionAsync(PermissionDto permission);
    }
}
