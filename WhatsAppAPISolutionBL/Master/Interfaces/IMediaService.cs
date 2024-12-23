using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediaService
    {
        Task<List<UMediaUpload>> GetMediaListAsync(int ClientId, string contentTypeStr = "", int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponseWithID> AddMediaAsync(MediaUploadDto media);
        Task<UResponseWithID> UploadMediaAsync(MediaFileDto media);
        Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media);
        Task<UResponseWithID> DeleteMediaAsync(int Id);
        Task<int> DownloadWhatsAppMediaToLocal(Client client, SenderName senderName, string mediaId);

        /// <summary>
        /// Get message type from media
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        Task<MessageTypeEnum> GetMessageTypeFromMedia(int mediaId);

        /// <summary>
        /// Check allowed media types
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        bool CheckAllowedMediaType(string extension);
    }
}
