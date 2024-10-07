using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IGroupService
    {
        public Task<List<UGroup>> GetGroupListAsync();
        public Task<UResponse> AddGroupAsync(GroupDto group);
        public Task<UResponse> UpdateGroupAsync(GroupDto group);
        public Task<UResponse> DeleteGroupAsync(int groupId);
    }
}
