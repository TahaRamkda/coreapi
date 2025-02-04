using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Client;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.SystemActions;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.Dto.SystemActions;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class SystemActionsService : ISystemActionsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public SystemActionsService(
            WhatsAppSolutionContext dbContext, 
            WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<USystemActions>> GetSystemActionsListAsync(int ClientId, int SystemActionId = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.SystemActions.FromSqlInterpolated($"exec usp_SystemActions_Ops @action_Id={(int)CrudEnum.List}, @ClientId={ClientId}, @SystemActionId={SystemActionId}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddSystemActionsAsync(int clientId, int userId, SystemActionsDto systemActions)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SystemActions_Ops @action_Id={(int)CrudEnum.Add}, @ClientId={clientId}, @SenderId={systemActions.SenderId}, @ActionId={systemActions.ActionId}, @ActionName={systemActions.ActionName}, @ThirdPartyURL={systemActions.ThirdPartyURL}, @ActionType={systemActions.ActionType}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateSystemActionsAsync(int clientId, int userId, SystemActionsDto systemActions)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SystemActions_Ops @action_Id={(int)CrudEnum.Update}, @SystemActionId={systemActions.SystemActionId}, @ClientId={clientId}, @SenderId={systemActions.SenderId}, @ActionId={systemActions.ActionId}, @ActionName={systemActions.ActionName}, @ThirdPartyURL={systemActions.ThirdPartyURL}, @ActionType={systemActions.ActionType}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteSystemActionsAsync(int systemActionsId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SystemActions_Ops @action_Id={(int)CrudEnum.Delete}, @SystemActionId={systemActionsId}").ToListAsync();
            return response[0];
        }

        public async Task<List<UEntityDto>> GetSystemActionsAsync(int clientId, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_SystemActions_Ops @action_Id={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<USystemActionsDetail> GetSystemActionsByIdAsync(int clientId,int id)
        {
            var response = await _dbContext2.SystemActionsDetail.FromSqlInterpolated($"exec usp_SystemActions_Ops @action_Id={(int)CrudEnum.GetById}, @ClientId={clientId}, @SystemActionId={id}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }
    }
}
