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

        public async Task<List<UAPIMessage>> GetAPIMessageListAsync(int clientId,int aPIMessageId = 0, int templateId = 0, int status = 0, string waID = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStr = "", string trxType = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
           // var query = string.Format(@"exec usp_APIMessages_Ops @ActionId={0}, @ClientId={1}, @TemplateId={2},@Status={3}, @WaID={4},@TrxType='{5}',, @From_Date='{4}', @To_Date='{5}', @SearchStr='{6}', @SortBy={7}, @PageNo={8}, @PageSize={9}", (int)CrudEnum.List, client_Id, order_Id, customer_Id, fromDate, toDate, searchStr, sortBy, pageNo, pageSize);
            
            var response = await _dbContext2.APIMessages.FromSqlInterpolated($"exec usp_APIMessages_Ops @ActionId={(int)CrudEnum.List}, @APIMessageId={aPIMessageId}, @ClientId={clientId}, @TemplateId={templateId}, @Status={status}, @WaID={waID}, @TrxType={trxType}, @SearchStr={searchStr}, @FromDate={fromDate}, @ToDate={toDate}, @SortBy={sortBy}, @PageNumber={pageNo}, @PageSize={pageSize}").ToListAsync();

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
