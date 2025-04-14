using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<DashboardService> _logger;
        private readonly ICacheService _cacheService;

        public DashboardService(WhatsAppSolutionContext2 dbContext2,
             ILogger<DashboardService> logger, ICacheService _cacheService)
        {
            _dbContext2 = dbContext2;
            _logger = logger;
            this._cacheService = _cacheService;
        }

        public async Task<string> GetDashboardSummaryAsync(int clientId, int senderId, DateTime? fromDate = null, DateTime? toDate = null)
        {

            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.JsonDatas.FromSqlInterpolated($"exec usp_Dashboard_Ops  @ClientId={clientId}, @SenderId={senderId}, @DashboardTypeId=1, @FromDate={fromDate}, @ToDate={toDate}").ToListAsync();
            _logger.LogInformation(
                "Calling procedure usp_Dashboard_Ops with clientId={ClientId}, senderId={SenderId}, fromDate={FromDate}, toDate={ToDate}, DashboardTypeId=1, ProcResponseTime={ProcResponseTime}ms", clientId, senderId, fromDate, toDate,DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds
            ); if (response != null && response.Any())
                return response[0].JsonDataStr;
            await _cacheService.RemoveAsync(CacheKeys.DASHBOARD_PATTERN_KEY);
            return String.Empty;
        }

        public async Task<string> GetTemplateInsightAsync(int clientId, int templateId = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.JsonDatas.FromSqlInterpolated($"exec usp_Template_Insight  @ClientId={clientId}, @TemplateId={templateId}, @FromDate={fromDate}, @ToDate={toDate}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Template_Insight with clientId={ClientId}, templateId={TemplateId}, fromDate={FromDate}, toDate={ToDate}, ProcResponseTime={ProcResponseTime}ms",clientId, templateId, fromDate, toDate,DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds
            ); if (response != null && response.Any())
                return response[0].JsonDataStr;
            await _cacheService.RemoveAsync(CacheKeys.DASHBOARD_PATTERN_KEY);
            return String.Empty;
        }
    }
}
