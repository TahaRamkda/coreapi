using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMessageService
    {
        Task<UResponse> UpdateMessageStatusAsync(WhatsAppMessageStatusUpdateDto messageStatus);

        /// <summary>
        /// Add message received logs
        /// </summary>
        /// <param name="messageReceive"></param>
        /// <returns></returns>
        Task<UMessageReceived> AddMessageReceivedLogAsync(WhatsAppMessageReceiveDto messageReceive);

        Task<UResponse> SendMessageAsync(SendMessageRequestDto model);
    }
}
