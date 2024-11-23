using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediaService
    {
        Task<List<UMediaUpload>> GetMediaListAsync(int ClientId);
        Task<UResponseWithID> AddMediaAsync(MediaUploadDto media);
        Task<UResponse> UploadMediaAsync(MediaUploadDto media);
        Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media);
        Task<UResponseWithID> DeleteMediaAsync(int Id);
        Task<long> DownloadWhatsAppMediaToLocal(Client client, SenderName senderName, string mediaId);
    }
}
