using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Media;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediaService
    {
        Task<List<UMediaUpload>> GetMediaListAsync(int clientId, int senderId = 0, string contentTypeStr = "", int pageNo = 0, int pageSize = int.MaxValue, int mediaTypeId = 0);
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
        /// Check allowed media types for message
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        bool CheckAllowedMediaTypeForMessage(string extension);

        /// <summary>
        /// Check allowed media types
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        bool CheckAllowedMediaType(string extension);

        bool CheckAllowedImageType(string extension);
        bool CheckAllowedVideoType(string extension);
        bool CheckAllowedDocumentType(string extension);

        /// <summary>
        /// Check allowed media header type
        /// </summary>
        /// <param name="headerType"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        bool CheckAllowedTemplateHeaderType(TemplateHeaderEnum headerType, string extension);

        void ExportCatalog(int clientId, int senderId, byte[] byteArray, string localization);
    }
}
