using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.SenderName;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class SenderNameService : ISenderNameService
    {
        private readonly int userId;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IUserService _userService;

        public SenderNameService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IUserService userService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _userService = userService;


            userId = _userService.GetUserIdFromAccessToken();
        }

        public async Task<List<USenderName>> GetSenderNameListAsync(int ClientId)
        {
            var response = await _dbContext2.SenderNames.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddSenderNameAsync(SenderNameDto senderName)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={senderName.ClientId}, @SenderName={senderName.SenderName}, @PhoneNumber={senderName.PhoneNumber}, @PhoneId={senderName.PhoneId}, @AppId={senderName.AppId}, @Limit={senderName.Limit}, @Quality={senderName.Quality}, @MediaId={senderName.MediaId}, @Verified={senderName.Verified}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateSenderNameAsync(SenderNameDto senderName)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.Update}, @SenderId={senderName.SenderId}, @ClientId={senderName.ClientId}, @SenderName={senderName.SenderName}, @PhoneNumber={senderName.PhoneNumber}, @PhoneId={senderName.PhoneId}, @AppId={senderName.AppId}, @Limit={senderName.Limit}, @Quality={senderName.Quality}, @MediaId={senderName.MediaId},@Verified={senderName.Verified}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteSenderNameAsync(int SenderNameId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.Delete}, @SenderId={SenderNameId}").ToListAsync();
            return response[0];
        }

        public async Task<List<UEntityDto>> GetSenderNamesAsync(int clientId, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<USenderNameDetail> GetSenderNameByIdAsync(int clientId, int senderId)
        {
            var response = await _dbContext2.SenderNameDetails.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}, @SenderId={senderId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }
    }
}
