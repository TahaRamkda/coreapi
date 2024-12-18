using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediaService
    {
        Task<List<UMediaUpload>> GetMediaListAsync(int ClientId, int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponseWithID> AddMediaAsync(MediaUploadDto media);
        Task<UResponseWithID> UploadMediaAsync(MediaFileDto media);
        Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media);
        Task<UResponseWithID> DeleteMediaAsync(int Id);
        Task<int> DownloadWhatsAppMediaToLocal(Client client, SenderName senderName, string mediaId);
    }
}
