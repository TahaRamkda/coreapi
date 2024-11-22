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
    public interface IRoleService
    {
        public Task<List<Role>> GetRoleListAsync(int ClientId);
        public Task<UResponse> AddRoleAsync(RoleDto role);
        public Task<UResponse> UpdateRoleAsync(RoleDto role);
        public Task<UResponse> DeleteRoleAsync(int RoleId, int ClientId);
    }
}
