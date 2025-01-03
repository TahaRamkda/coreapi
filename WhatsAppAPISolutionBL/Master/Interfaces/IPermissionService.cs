using WhatsAppAPISolutionDL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Permission;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.Dto.User;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IPermissionService
    {
        public Task<List<UPermissionList>> GetPermissionListAsync(int ClientId, int RoleId = 0);
        public Task<UResponse> AddPermissionAsync(PermissionDto permission);
    }
}
