using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
    public class APIMessageService: IAPIMessageService
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
            var response = await _dbContext2.APIMessages.FromSqlInterpolated($"exec usp_APIMessages_Ops @ActionId={(int)CrudEnum.List}, @APIMessageId={APIMessageId}, @ClientId={ClientId}, @TemplateId={TemplateId}, @Status={Status}, @WaID={WaID}, @TrxType={TrxType}, @SearchStr={SearchStr}, @FromDate={FromDate}, @ToDate={ToDate}, @SortBy={SortBy}, @PageNumber={PageNo}, @PageSize={PageSize}").ToListAsync();

            return response;
        }
        public async Task<UResponse> AddAPIMessageAsync(APIMessageDto apiMesage)
        {
            var query = string.Format(@"exec usp_APIMessages_Ops @ActionId={0}, @TrxType='{1}', @UDF1='{2}', @UDF2='{3}', @TemplateId={4}, @ClientId={5}, @URL='{6}', @Status='{7}', @SenderNameId={8}, @WaID='{9}', @PhoneNumber='{10}', @ScheduleTime='{11}', @ActionBy={12}", (int)CrudEnum.Add, apiMesage.TrxType, apiMesage.Udf1, apiMesage.Udf2, apiMesage.TemplateId, apiMesage.ClientId, apiMesage.Url, apiMesage.Status, apiMesage.SenderNameId, apiMesage.WaId, apiMesage.PhoneNumber, apiMesage.ScheduleTime, apiMesage.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
