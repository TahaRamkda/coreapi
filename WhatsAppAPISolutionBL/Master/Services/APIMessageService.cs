using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class APIMessageService : IAPIMessageService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public APIMessageService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UAPIMessage>> GetAPIMessageListAsync(int ClientId, int APIMessageId = 0, int TemplateId = 0, int Status = 0, string WaID = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", string TrxType = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.APIMessages.FromSqlInterpolated($"exec usp_APIMessages_Ops @ActionId={(int)CrudEnum.List}, @APIMessageId={APIMessageId}, @ClientId={ClientId}, @TemplateId={TemplateId}, @Status={Status}, @WaID={WaID}, @TrxType={TrxType}, @SearchStr={SearchStr ?? ""}, @FromDate={FromDate}, @ToDate={ToDate}, @SortBy={SortBy}, @PageNumber={PageNo}, @PageSize={PageSize}").ToListAsync();

            return response;
        }
        public async Task<UResponseWithID> AddAPIMessageAsync(APIMessageDto apiMesage)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_APIMessages_Ops @ActionId={(int)CrudEnum.Add}, @TrxType={apiMesage.TrxType}, @UDF1={apiMesage.UDF1}, @UDF2={apiMesage.UDF2}, @TemplateId={apiMesage.TemplateId}, @ClientId={apiMesage.ClientId}, @URL={apiMesage.Url}, @Status={apiMesage.Status}, @SenderNameId={apiMesage.SenderNameId}, @WaID={apiMesage.WaId}, @PhoneNumber={apiMesage.PhoneNumber}, @ScheduleTime={apiMesage.ScheduleTime}, @ActionBy={apiMesage.ActionBy}").ToListAsync();
            return response[0];
        }
    }
}
