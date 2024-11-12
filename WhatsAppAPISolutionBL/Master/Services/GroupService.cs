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

        public async Task<List<UGroup>> GetGroupListAsync(int ClientId)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, ClientId);
            var response = await _dbContext2.Groups.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddGroupAsync(GroupDto group)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @ClientId={1}, @GroupName='{2}', @ActionBy={3}", (int)CrudEnum.Add, group.ClientId, group.GroupName, group.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateGroupAsync(GroupDto group)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @ClientId={1}, @GroupId={2}, @GroupName='{3}', @ActionBy={4}", (int)CrudEnum.Update, group.ClientId, group.GroupId, group.GroupName, group.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteGroupAsync(int GroupId)
        {
            var query = string.Format(@"exec usp_Groups_Ops @ActionId={0}, @GroupId={1}", (int)CrudEnum.Delete, GroupId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
