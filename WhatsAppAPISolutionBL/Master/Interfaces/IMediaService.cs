using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediaService
    {
        public Task<List<UMediaUpload>> GetMediaListAsync(int client_Id);
        public Task<UResponseWithID> AddMediaAsync(MediaUploadDto media);
        public Task<UResponse> UploadMediaAsync(MediaUploadDto media);
        public Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media);
        public Task<UResponseWithID> DeleteMediaAsync(int id);
    }
}
