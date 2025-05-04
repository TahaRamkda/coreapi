using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediatorService
    {
        /// <summary>
        /// Centralized function to process all type of DB response
        /// </summary>
        /// <param name="dBResponse"></param>
        /// <returns></returns>
        Task<ApiResult> ProcessDBResponse(int clientId, int senderId, DBResponse model);
    }
}
