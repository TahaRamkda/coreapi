using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MessageService : IMessageService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IMediaService _mediaService;

        public MessageService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IMediaService mediaService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _mediaService = mediaService;
        }

        public async Task<UResponse> UpdateMessageStatusAsync(WhatsAppMessageStatusUpdateDto messageStatus)
        {
            var eventType = 0;
            var eventStatus = 1;
            var conversationId = "";
            var eventMessage = "";
            var pricingModel = "";
            var billable = false;
            var category = "";
            long senderId = 0;

            if (messageStatus.phone_number_Id != null)
            {
                if (!string.IsNullOrEmpty(messageStatus.phone_number_Id.display_phone_number) && !string.IsNullOrEmpty(messageStatus.phone_number_Id.phone_number_id))
                {
                    var senderName = await _dbContext.SenderNames.Where(x => x.ClientId == Convert.ToInt32(messageStatus.client_Id) && x.PhoneNumberId == messageStatus.phone_number_Id.phone_number_id).FirstOrDefaultAsync();
                    if (senderName != null)
                        senderId = senderName.SenderId;
                }
            }

            if (messageStatus.status.ToLower() == MessageStatusEnum.SENT.ToString().ToLower())
                eventType = 1;
            else if (messageStatus.status.ToLower() == MessageStatusEnum.DELIVERED.ToString().ToLower())
                eventType = 2;
            else if (messageStatus.status.ToLower() == MessageStatusEnum.READ.ToString().ToLower())
                eventType = 3;
            else if (messageStatus.status.ToLower() == MessageStatusEnum.FAILED.ToString().ToLower())
            {
                eventType = 4;
                eventStatus = 0;
            }

            if (messageStatus.conversation != null)
                conversationId = messageStatus.conversation.id;

            if (messageStatus.error != null)
                eventMessage = messageStatus.error.error_Details;

            if (messageStatus.pricing != null)
            {
                pricingModel = messageStatus.pricing.pricing_model;
                billable = messageStatus.pricing.billable;
                category = messageStatus.pricing.category;
            }

            var query = string.Format(@"exec usp_MessageSentLogs_StatusUpdate @ModuleId={0}, @ClientId={1}, @ParentId={2}, @SenderId={3}, @PhoneNumber='{4}', @WaId='{5}', @WaId2='{6}', @EventType={7}, @EventTime='{8}', @EventStatus={9}, @EventMessage='{10}', @PricingModel='{11}', @Billable={12}, @Category='{13}'", 0, messageStatus.client_Id, 0, senderId, messageStatus.recipient_Id, messageStatus.wam_Id, conversationId, eventType, messageStatus.update_dateTime, eventStatus, eventMessage, pricingModel, billable, category);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        /// <summary>
        /// Add message received logs
        /// </summary>
        /// <param name="messageReceive"></param>
        /// <returns></returns>
        public async Task<UMessageReceived> AddMessageReceivedLogAsync(WhatsAppMessageReceiveDto messageReceive)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == Convert.ToInt64(messageReceive.client_Id));
            var senderName = await _dbContext.SenderNames.FirstOrDefaultAsync(x => x.PhoneNumberId == messageReceive.phone_number_Id.phone_number_id);

            int messageType = 0;
            string messageText = String.Empty;
            long mediaId = 0;
            messageReceive.type = messageReceive.type.ToUpper();

            if (messageReceive.type == MessageReceiveTypeEnum.BUTTON.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.BUTTON);
                messageText = messageReceive.button.payload;
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.TEXT.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.TEXT);
                messageText = messageReceive.text.body;
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.IMAGE.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.IMAGE);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.image.id);
                messageText = messageReceive.image.caption;
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.VIDEO.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.VIDEO);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.video.id);
                messageText = messageReceive.video.caption;
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.DOCUMENT.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.DOCUMENT);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.document.id);
                messageText = messageReceive.document.caption;
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.LOCATION.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.LOCATION);
                messageText = String.Concat(messageReceive.location.latitude, ",", messageReceive.location.longitude);
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.STICKER.ToString())
            {
                messageType = Convert.ToInt32(MessageReceiveTypeEnum.STICKER);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.sticker.id);
                messageText = messageReceive.sticker.caption;
            }

            var response = await _dbContext2.UMessageReceiveds.FromSqlInterpolated($"exec usp_MessageReceivedLogs_ops @ClientId={messageReceive.client_Id}, @SenderId={senderName?.SenderId}, @WaId={messageReceive.wam_Id}, @ContextWaId={messageReceive.context?.wam_Id}, @PhoneNumber={messageReceive.from}, @ResponseType={messageType}, @ResponseText={messageText}, @MediaId={mediaId}").ToListAsync();

            //Central service call
            if (response != null & response.Any())
            {

            }

            return response != null && response.Any() ? response[0] : null;
        }

        public async Task<UResponse> SendMessageAsync(SendMessageRequestDto model)
        {
            model.Message = model.Message.Trim();
            model.PhoneNumbers = model.PhoneNumbers.Where(x => !String.IsNullOrWhiteSpace(x)).Select(x => x.Replace("+", "").Trim()).ToList();

            return new UResponse();
        }
    }
}
