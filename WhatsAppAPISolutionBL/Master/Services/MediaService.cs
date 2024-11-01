using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MediaService : IMediaService
    {
        private readonly string _uploadPath;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<MediaService> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public MediaService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
          IHttpClientFactory httpClientFactory,
          IWebHostEnvironment hostingEnvironment,
          ILogger<MediaService> logger,
          IHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _hostingEnvironment = hostingEnvironment;
            _httpClient = httpClientFactory.CreateClient("bridge_api");
            _logger = logger;
            _hostEnvironment = hostEnvironment;

            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
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
        public async Task<UResponse> UploadMediaAsync(MediaUploadDto model)
        {
            var senderName = await _dbContext.SenderNames.Where(x => x.SenderId == model.Sender_Name_Id).FirstOrDefaultAsync();
            if (senderName == null)
                return new UResponse()
                {
                    Status = 0,
                    Message = "Sender name not exist"
                };
            if (model.File != null && model.File.Length > 0)
            {
                // Check file size (20 MB = 50 * 1024 * 1024 bytes)
                const long maxFileSize = 20 * 1024 * 1024; // 20 MB
                if (model.File.Length > maxFileSize)
                {
                    return new UResponse()
                    {
                        Status = 0,
                        Message = "File size must not exceed 20 MB."
                    };
                }

                var originalFileName = model.File.FileName.Replace(" ", "_");
                var filePath = Path.Combine(_uploadPath, originalFileName);

                // Check if the file already exists and create a unique filename if it does
                var fileExtension = Path.GetExtension(originalFileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                int counter = 1;

                while (System.IO.File.Exists(filePath))
                {
                    var newFileName = $"{fileNameWithoutExtension}_{counter}{fileExtension}";
                    filePath = Path.Combine(_uploadPath, newFileName);
                    counter++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var fileUrl = Path.Combine("https://whatsappapi.consulttechies.com", "Uploads", Path.GetFileName(filePath));
                if (!string.IsNullOrEmpty(fileUrl))
                    fileUrl = fileUrl.Replace("\\", "/");

                // Create response details
                var media = new MediaUploadDto()
                {
                    Sender_Name_Id = model.Sender_Name_Id,
                    Client_Id = model.Client_Id,
                    File_Name = Path.GetFileName(filePath),
                    File_Size = model.File.Length,
                    File_Extension = fileExtension,
                    Content_Type = model.File.ContentType,
                    Media_Path = fileUrl,
                    ActionBy = model.ActionBy
                };
                var insMedia = await AddMediaAsync(media);
                if (insMedia != null && insMedia.Id > 0)
                {
                    var mediaUpload = new MediaUploadBridgeDto()
                    {
                        clientId = model.Client_Id.ToString(),
                        senderNameId = senderName.SenderId.ToString()
                    };
                    mediaUpload.medias.Add(new MediaUploadBridgeDto.Media()
                    {
                        id = insMedia.Id.ToString(),
                        url = fileUrl,
                    });
                    var requestStr = JsonConvert.SerializeObject(mediaUpload);

                    var response = await _httpClient.PostAsync($"/api/Upload/UploadMedia", new StringContent(requestStr, null, "application/json"));
                    var content = await response.Content.ReadAsStringAsync();

                    var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
                    if (result != null && result.success)
                    {
                        var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                        var mediaResult = JsonConvert.DeserializeObject<List<MediaResultDto>>(data);
                        if (mediaResult != null && mediaResult.Any())
                        {
                            if (!string.IsNullOrEmpty(mediaResult[0].mediaId))
                            {
                                var updateDto = new MediaUploadDto()
                                {
                                    Id = Convert.ToInt64(mediaResult[0].id),
                                    Media_Id = mediaResult[0].mediaId
                                };
                                var updateMedia = await UpdateMediaAsync(updateDto);
                                if (updateMedia == null || updateMedia.Status <= 0)
                                {
                                    return new UResponse()
                                    {
                                        Status = 0,
                                        Message = updateMedia?.Message
                                    };
                                }
                                return new UResponse()
                                {
                                    Status = 1,
                                    Message = "Media added successfully"
                                };
                            }
                        }
                    }
                    return new UResponse()
                    {
                        Status = 0,
                        Message = "oops something went wrong"
                    };
                }
                return new UResponse()
                {
                    Status = 0,
                    Message = "oops something went wrong"
                };
            }
            return new UResponse()
            {
                Status = 0,
                Message = "oops something went wrong"
            };
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
