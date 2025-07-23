using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Bridge;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        #region Fields
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly ILogger<UserPreferenceService> _logger;
        private readonly ISenderNameService _senderNameService;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        #endregion

        #region Ctor
        public UserPreferenceService(
            ILogger<UserPreferenceService> loogger,
            ISenderNameService senderNameService,
            WhatsAppSolutionContext2 dbContext2)
        {
            _logger = loogger;
            _senderNameService = senderNameService;
            _dbContext2 = dbContext2;
        }
        #endregion

        #region Method
        public async Task<ApiResult> Processuserpreference(UserPreferenceDto Dto)
        {
            _logger.LogInformation("ProcessTemplateAnalyticsAsync called with model: {model}", JsonConvert.SerializeObject(Dto));
            if (Dto == null || string.IsNullOrEmpty(Dto.Phone_Number))
            {
                return new ApiResult
                {
                    Message = "PhoneNumber should not be empty."
                };
            }
            int senderId = 0;

            if (Dto.Phone_Number_Id != null
                && !String.IsNullOrEmpty(Dto.Phone_Number_Id.display_phone_number)
                && !String.IsNullOrEmpty(Dto.Phone_Number_Id.phone_number_id))
            {
                var senderName = await _senderNameService.GetSenderNameEntityByPhoneNumberIdAsync(Dto.Phone_Number_Id.phone_number_id);
                if (senderName != null)
                    senderId = senderName.SenderId;
            }
            _logger.LogInformation("SenderId Fetched {senderId}:", senderId);
            int actionId = Dto.Value ? (int)UserPreferenceEnum.Resume : (int)UserPreferenceEnum.Stop;
            var result = await _dbContext2.UserPreferences.FromSqlInterpolated($"Exec usp_UnsubscribedNumbers_Ops @ActionId={actionId}, @SenderId={senderId}, @ClientId={Dto.Client_Id}, @BlockType={Dto.Comments}, @PhoneNumber={Dto.Phone_Number}").ToListAsync();
            var finalResult = result.FirstOrDefault();
            return new ApiResult(){
                Result = finalResult,
                StatusCode =finalResult.Status
            };
        }
        #endregion
    }
}
