using Microsoft.EntityFrameworkCore;
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
    public class MediaService : IMediaService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public MediaService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UMediaUpload>> GetMediaListAsync(int client_Id)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Client_Id={1}", (int)CrudEnum.List, client_Id);
            var response = await _dbContext2.UMediaUploads.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddMediaAsync(MediaUploadDto media)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Client_Id={1}, @WhatsApp_BusinessAccount_Id='{2}', @Sender_Name_Id={3}, @Media_Path='{4}', @Action_By={5}", (int)CrudEnum.Add, media.Client_Id, media.WhatsApp_BusinessAccount_Id, media.Sender_Name_Id, media.Media_Path, media.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateMediaAsync(MediaUploadDto media)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Id={1}, @Client_Id={2}, @Media_URL='{3}', @Media_Id='{4}', @Action_By={5}", (int)CrudEnum.Update, media.Id, media.Client_Id, media.Media_Url, media.Media_Id, media.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteMediaAsync(int media_Id)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Id={1}", (int)CrudEnum.Delete, media_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
