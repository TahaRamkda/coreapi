using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Media;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MediaService : IMediaService
    {
        #region Fields

        private readonly string _uploadPath;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MediaService> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;

        #endregion

        #region Ctor

        public MediaService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            ILogger<MediaService> logger,
            IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _logger = logger;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;

            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        #endregion

        #region Methods

        public async Task<List<UMediaUpload>> GetMediaListAsync(int ClientId, string contentTypeStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.UMediaUploads.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @ContentTypeStr={contentTypeStr}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponseWithID> AddMediaAsync(MediaUploadDto media)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={media.ClientId}, @WhatsAppBusinessAccountId={media.WhatsAppBusinessAccountId}, @SenderNameId={media.SenderNameId}, @MediaPath={media.MediaPath}, @ContentType={media.ContentType}, @FileSize={media.FileSize}, @FileName={media.FileName}, @FileExtension={media.FileExtension}, @MediaSourceId={media.MediaSourceId}, @ActionBy={media.ActionBy}, @MediaId={media.MediaId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponseWithID> UploadMediaAsync(MediaFileDto model)
        {
            var senderName = await _dbContext.SenderNames.Where(x => x.SenderId == model.SenderNameId).FirstOrDefaultAsync();
            if (model.UploadToFacebook && senderName == null)
                return new UResponseWithID
                {
                    Status = 0,
                    Message = "Sender name not exist"
                };

            if (model.File != null && model.File.Length > 0)
            {
                // Check file size (20 MB = 20 * 1024 * 1024 bytes)
                int maxFileSize = (_apiSolutionConfigurationSettings.Value.MaxFileSizeInMB == 0 ? 20 : _apiSolutionConfigurationSettings.Value.MaxFileSizeInMB);
                int maxFileLength = maxFileSize * 1024 * 1024; // 20 MB

                if (model.File.Length > maxFileLength)
                {
                    return new UResponseWithID
                    {
                        Status = 0,
                        Message = "File size must not exceed 20 MB."
                    };
                }

                var originalFileName = model.File.FileName.Replace(" ", "_").Trim();
                var filePath = Path.Combine(_uploadPath, originalFileName);

                var fileExtension = Path.GetExtension(originalFileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                int counter = 1;

                // Check if the file already exists and create a unique filename if it does
                while (File.Exists(filePath))
                {
                    var newFileName = $"{fileNameWithoutExtension}_{counter}{fileExtension}";
                    filePath = Path.Combine(_uploadPath, newFileName);
                    counter++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var absolutePath = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, _apiSolutionConfigurationSettings.Value.StaticFolderPath);
                var mediaPath = Path.Combine(_apiSolutionConfigurationSettings.Value.StaticFolderPath, Path.GetFileName(filePath));
                var fileUrl = Path.Combine(absolutePath, Path.GetFileName(filePath));
                if (!string.IsNullOrEmpty(fileUrl))
                    fileUrl = fileUrl.Replace("\\", "/");

                // Create response details
                var media = new MediaUploadDto
                {
                    SenderNameId = model.SenderNameId,
                    ClientId = model.ClientId,
                    FileName = Path.GetFileName(filePath),
                    FileSize = (int)model.File.Length,
                    FileExtension = fileExtension,
                    ContentType = model.File.ContentType,
                    MediaPath = mediaPath,
                    MediaSourceId = model.MediaSourceId,
                    ActionBy = model.ActionBy
                };

                var insMedia = await AddMediaAsync(media);
                if (insMedia != null && insMedia.Id > 0)
                {
                    //If upload media to facebook
                    if (model.UploadToFacebook)
                        return await UploadMediaToFacebook(media, insMedia.Id, fileUrl);

                    return new UResponseWithID
                    {
                        Status = 1,
                        Id = insMedia.Id,
                        Message = "Media added successfully"
                    };
                }

                return new UResponseWithID()
                {
                    Status = 0,
                    Message = "Something went wrong, cannot upload media right now"
                };
            }

            return new UResponseWithID()
            {
                Status = 0,
                Message = "Something went wrong, cannot upload media right now"
            };
        }

        public async Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.Update}, @Id={media.Id}, @MediaId={media.MediaId}, @ActionBy={media.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponseWithID> DeleteMediaAsync(int Id)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.Delete}, @Id={Id}").ToListAsync();
            return response[0];
        }

        /// <summary>
        /// Download media to local
        /// </summary>
        /// <param name="client"></param>
        /// <param name="senderName"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public async Task<int> DownloadWhatsAppMediaToLocal(Client client, SenderName senderName, string mediaId)
        {
            try
            {
                GetMediaResult mediaResult = null;
                using (var httpClient = new HttpClient())
                {
                    // Step 1: Get the media information
                    string mediaInfoUrl = $"{_bridgeConfigurationSettings.Value.WhatsappBaseURL}/{mediaId}";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", client?.AccessToken);

                    var mediaInfoResponse = await httpClient.GetAsync(mediaInfoUrl);
                    if (!mediaInfoResponse.IsSuccessStatusCode)
                        return 0;

                    var responseStr = await mediaInfoResponse.Content.ReadAsStringAsync();
                    mediaResult = JsonConvert.DeserializeObject<GetMediaResult>(responseStr);
                }

                if (mediaResult != null)
                {
                    using (var httpClient = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, mediaResult.url);
                        request.Headers.Add("Authorization", $"Bearer {client?.AccessToken}");
                        request.Headers.Add("User-Agent", $"node");
                        var mediaResponse = await httpClient.SendAsync(request);
                        if (!mediaResponse.IsSuccessStatusCode)
                            return 0;

                        string contentDisposition = mediaResponse.Content.Headers.ContentDisposition?.FileName ?? $"media_{mediaId}";
                        string fileExtension = Path.GetExtension(contentDisposition) ?? ".dat";
                        string fileName = $"media_{mediaId}{fileExtension}";
                        string localFilePath = Path.Combine(_uploadPath, fileName);

                        if (File.Exists(localFilePath))
                            File.Delete(localFilePath);

                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await mediaResponse.Content.CopyToAsync(fileStream);
                        }

                        string absoluteFilePath = _apiSolutionConfigurationSettings.Value.StaticFolderPath;
                        var media = await AddMediaAsync(new MediaUploadDto
                        {
                            ClientId = client.ClientId,
                            SenderNameId = senderName.SenderId,
                            ContentType = mediaResult.mime_type,
                            FileExtension = Path.GetExtension(localFilePath),
                            FileName = Path.GetFileName(localFilePath),
                            FileSize = mediaResult.file_size,
                            MediaId = mediaId,
                            MediaPath = String.Concat(absoluteFilePath, Path.GetFileName(localFilePath)),
                            WhatsAppBusinessAccountId = senderName.BusinessAccountId,
                            MediaSourceId = (int)MediaSourceEnum.Conversation
                        });

                        if (media != null)
                            return media.Id;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// Get message type from media
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public async Task<MessageTypeEnum> GetMessageTypeFromMedia(int mediaId)
        {
            var media = await _dbContext.Medias.FindAsync(mediaId);
            if (media == null || String.IsNullOrEmpty(media.MediaId) || String.IsNullOrWhiteSpace(media.FileExtension))
                return MessageTypeEnum.TEXT;

            MessageTypeEnum type = MessageTypeEnum.TEXT;
            switch (media.FileExtension.ToLower())
            {
                case ".aac":
                case ".amr":
                case ".mp3":
                case ".m4a":
                case ".ogg":
                    type = MessageTypeEnum.AUDIO;
                    break;
                case ".txt":
                case ".xls":
                case ".xlsx":
                case ".doc":
                case ".docx":
                case ".ppt":
                case ".pptx":
                case ".pdf":
                    type = MessageTypeEnum.DOCUMENT;
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    type = MessageTypeEnum.IMAGE;
                    break;
                case ".webp":
                case ".3gp":
                case ".mp4":
                    type = MessageTypeEnum.VIDEO;
                    break;
                default: break;
            }

            return type;
        }

        /// <summary>
        /// Check allowed media types
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool CheckAllowedMediaType(string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            List<string> allowedMediaExtensions = new List<string>();
            allowedMediaExtensions.AddRange(new List<string> { ".aac", ".amr", ".mp3", ".m4a", ".ogg" }); //Audio types
            allowedMediaExtensions.AddRange(new List<string> { ".txt", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".pdf" }); //Document types
            allowedMediaExtensions.AddRange(new List<string> { ".jpg", ".jpeg", ".png" }); // Image types
            allowedMediaExtensions.AddRange(new List<string> { ".webp", ".3gp", ".mp4" }); // Video types

            return allowedMediaExtensions.Contains(extension.ToLower());
        }

        public bool CheckAllowedTemplateHeaderType(TemplateHeaderEnum headerType, string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            if (headerType == TemplateHeaderEnum.IMAGE)
                return new List<string> { ".jpg", ".jpeg", ".png" }.Contains(extension.ToLower());
            else if (headerType == TemplateHeaderEnum.VIDEO)
                return new List<string> { ".webp", ".3gp", ".mp4" }.Contains(extension.ToLower());
            else if (headerType == TemplateHeaderEnum.DOCUMENT)
                return new List<string> { ".txt", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".pdf" }.Contains(extension.ToLower());

            return false;
        }

        private async Task<UResponseWithID> UploadMediaToFacebook(MediaUploadDto model, int mediaId, string fileUrl)
        {
            var mediaUpload = new MediaUploadBridgeDto
            {
                clientId = model.ClientId.ToString(),
                senderNameId = model.SenderNameId.ToString(),
                medias = new List<MediaUploadBridgeDto.Media> {
                    new MediaUploadBridgeDto.Media {
                        id = mediaId.ToString(),
                        url = fileUrl
                    }
                }
            };

            var requestStr = JsonConvert.SerializeObject(mediaUpload);

            var response = await _httpClient.PostAsync($"/api/Upload/UploadMedia", new StringContent(requestStr, null, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var mediaResult = JsonConvert.DeserializeObject<List<MediaResultDto>>(data);
                if (mediaResult != null && mediaResult.Any())
                {
                    if (!string.IsNullOrEmpty(mediaResult[0].mediaId))
                    {
                        var updateDto = new MediaUploadDto
                        {
                            Id = Convert.ToInt32(mediaResult[0].id),
                            MediaId = mediaResult[0].mediaId
                        };

                        var updateMedia = await UpdateMediaAsync(updateDto);
                        if (updateMedia == null || updateMedia.Status <= 0)
                        {
                            return new UResponseWithID
                            {
                                Status = 0,
                                Message = updateMedia?.Message
                            };
                        }
                        else
                        {
                            return new UResponseWithID
                            {
                                Status = 1,
                                Id = updateDto.Id,
                                Message = "Media added successfully"
                            };
                        }
                    }

                    return new UResponseWithID()
                    {
                        Status = 1,
                        Message = "Media added but unable to get media id from facebook"
                    };
                }
            }

            return new UResponseWithID()
            {
                Status = 0,
                Message = "Something went wrong, cannot upload media right now"
            };
        }

        #endregion
    }
}
