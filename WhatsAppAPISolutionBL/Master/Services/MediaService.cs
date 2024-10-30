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
        public async Task<UResponseWithID> AddMediaAsync(MediaUploadDto media)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Client_Id={1}, @WhatsApp_BusinessAccount_Id='{2}', @Sender_Name_Id={3}, @Media_Path='{4}', @Content_Type='{5}', @File_Size='{6}', @File_Name='{7}', @File_Extension='{8}', @Action_By={9}", (int)CrudEnum.Add, media.Client_Id, media.WhatsApp_BusinessAccount_Id, media.Sender_Name_Id, media.Media_Path, media.Content_Type, media.File_Size, media.File_Name, media.File_Extension, media.ActionBy);
            var response = await _dbContext2.ResponseWithID.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Id={1}, @Media_Id='{2}', @Action_By={3}", (int)CrudEnum.Update, media.Id, media.Media_Id, media.ActionBy);
            var response = await _dbContext2.ResponseWithID.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponseWithID> DeleteMediaAsync(int id)
        {
            var query = string.Format(@"exec usp_Media_Ops @ActionId={0}, @Id={1}", (int)CrudEnum.Delete, id);
            var response = await _dbContext2.ResponseWithID.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
