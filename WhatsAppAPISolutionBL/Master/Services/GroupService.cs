using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class GroupService: IGroupService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public GroupService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UGroup>> GetGroupListAsync()
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.Groups.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddGroupAsync(GroupDto group)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @Group_Name='{1}', @Action_By={2}", (int)CrudEnum.Add, group.Group_Name, group.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateGroupAsync(GroupDto group)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @Group_Id={1}, @Group_Name='{2}', @Action_By={3}", (int)CrudEnum.Update, group.Group_Id, group.Group_Name, group.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteGroupAsync(int group_Id)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @Group_Id={1}", (int)CrudEnum.Delete, group_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
