using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections;
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

        private readonly int userId;
        private readonly string _uploadPath;
        private readonly string _staticFolderPath;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MediaService> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly IUserService _userService;
        private readonly List<string> _allowedImageExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
        private readonly List<string> _allowedVideoExtensions = new List<string> { ".webp", ".3gp", ".mp4" };
        private readonly List<string> _allowedDocumentExtensions = new List<string> { ".txt", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".pdf" };
        private readonly List<string> _allowedAudioExtensions = new List<string> { ".aac", ".amr", ".mp3", ".m4a", ".ogg" };

        #endregion

        #region Ctor

        public MediaService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            ILogger<MediaService> logger,
            IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            IWebHostEnvironment webHostEnvironment,
            IUserService userService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _logger = logger;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _userService = userService;

            _staticFolderPath = _apiSolutionConfigurationSettings.Value.StaticFolderPath;
            _uploadPath = webHostEnvironment.ContentRootPath.TrimEnd('\\');

            if (!Directory.Exists(Path.Combine(_uploadPath, _staticFolderPath)))
                Directory.CreateDirectory(Path.Combine(_uploadPath, _staticFolderPath));

            userId = _userService.GetUserIdFromAccessToken();
        }

        #endregion

        #region Utilities

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
                            return new UResponseWithID { Message = updateMedia?.Message };
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

                    return new UResponseWithID { Message = "Media added but unable to get media id from facebook" };
                }
            }

            return new UResponseWithID { Message = "Something went wrong, cannot upload media right now" };
        }

        private int GetMediaTypeIdFromExtension(string fileExtension)
        {
            if (String.IsNullOrWhiteSpace(fileExtension))
                return 0;

            fileExtension = fileExtension.ToLower();
            int type = 0;
            if (_allowedImageExtensions.Contains(fileExtension))
                type = (int)MediaTypeEnum.IMAGE;
            else if (_allowedVideoExtensions.Contains(fileExtension))
                type = (int)MediaTypeEnum.VIDEO;
            else if (_allowedDocumentExtensions.Contains(fileExtension))
                type = (int)MediaTypeEnum.DOCUMENT;
            else if (_allowedAudioExtensions.Contains(fileExtension))
                type = (int)MediaTypeEnum.AUDIO;

            return type;
        }

        #endregion

        #region Methods

        public async Task<List<UMediaUpload>> GetMediaListAsync(int clientId, int senderId = 0, string contentTypeStr = "", int PageNo = 0, int PageSize = int.MaxValue, int mediaTypeId = 0)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.UMediaUploads.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @SenderNameId={senderId}, @ContentTypeStr={contentTypeStr}, @PageNo={PageNo}, @PageSize={PageSize}, @MediaTypeId={mediaTypeId}").ToListAsync();
            _logger.LogDebug("Calling procedure usp_Medias_Ops with actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", (int)CrudEnum.List, CrudEnum.List, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<UResponseWithID> AddMediaAsync(MediaUploadDto media)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={media.ClientId}, @WhatsAppBusinessAccountId={media.WhatsAppBusinessAccountId}, @SenderNameId={media.SenderNameId}, @MediaPath={media.MediaPath}, @ContentType={media.ContentType}, @FileSize={media.FileSize}, @FileName={media.FileName}, @FileExtension={media.FileExtension}, @MediaSourceId={media.MediaSourceId}, @ActionBy={userId}, @MediaId={media.MediaId}, @MediaTypeId={media.MediaTypeId}").ToListAsync();
            _logger.LogDebug("Calling procedure usp_Medias_Ops with actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", (int)CrudEnum.Add, CrudEnum.Add, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response[0];
        }

        public async Task<UResponseWithID> UploadMediaAsync(MediaFileDto model)
        {
            var senderName = await _dbContext.SenderNames.Where(x => x.SenderId == model.SenderNameId).FirstOrDefaultAsync();
            if (model.UploadToFacebook && senderName == null)
                return new UResponseWithID { Message = "Sender name not exist" };

            if (model.File != null && model.File.Length > 0)
            {
                var originalFileName = model.File.FileName.Replace(" ", "_").Trim();
                var fileExtension = Path.GetExtension(originalFileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                var mediaTypeId = GetMediaTypeIdFromExtension(fileExtension);
                int maxFileSize = 1; //Allow atleast 1 mb files

                string keyNames = string.Join(",", new[] { MediaSizeEnum.ImageSizeInMB.ToString(), MediaSizeEnum.VideoSizeInMB.ToString(), MediaSizeEnum.DocumentSizeInMB.ToString(), MediaSizeEnum.AudioSizeInMB.ToString() });
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={model.ClientId}, @SenderId={model.SenderNameId}").ToListAsync();
                _logger.LogDebug("Calling procedure usp_Appsettings_Ops with actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", (int)CrudEnum.GetAppSettings, CrudEnum.GetAppSettings, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

                // Determine the key name based on the media type
                string keyName = mediaTypeId switch
                {
                    (int)MediaTypeEnum.IMAGE => MediaSizeEnum.ImageSizeInMB.ToString(),
                    (int)MediaTypeEnum.VIDEO => MediaSizeEnum.VideoSizeInMB.ToString(),
                    (int)MediaTypeEnum.DOCUMENT => MediaSizeEnum.DocumentSizeInMB.ToString(),
                    (int)MediaTypeEnum.AUDIO => MediaSizeEnum.AudioSizeInMB.ToString(),
                    _ => null
                };

                if (!string.IsNullOrEmpty(keyName))
                {
                    var mediaSize = response.FirstOrDefault(x => x.KeyName == keyName);
                    if (mediaSize != null)
                        maxFileSize = Convert.ToInt32(mediaSize.Val);
                }

                // Check file size
                int maxFileLength = maxFileSize * 1024 * 1024;

                if (model.File.Length > maxFileLength)
                    return new UResponseWithID { Message = $"File size must not exceed {maxFileSize} MB." };

                // Determine media type folder name
                string mediaTypeFolder = String.Empty;

                if (model.MediaSourceId == (int)MediaSourceEnum.Conversation)
                    mediaTypeFolder = Path.Combine(MediaSourceEnum.Conversation.ToString(), Enum.GetName(typeof(MediaTypeEnum), mediaTypeId));
                else if (model.MediaSourceId == (int)MediaSourceEnum.Admin)
                    mediaTypeFolder = Path.Combine(MediaSourceEnum.Admin.ToString(), Enum.GetName(typeof(MediaTypeEnum), mediaTypeId));

                // Create folder path: uploads/clientId/senderId/mediaType
                string mediaFolder = _staticFolderPath;
                if (model.ClientId > 0)
                    mediaFolder = Path.Combine(mediaFolder, model.ClientId.ToString());

                if (model.SenderNameId > 0)
                    mediaFolder = Path.Combine(mediaFolder, model.SenderNameId.ToString());

                mediaFolder = Path.Combine(mediaFolder, mediaTypeFolder);

                // Ensure directories exist
                if (!Directory.Exists(Path.Combine(_uploadPath, mediaFolder)))
                    Directory.CreateDirectory(Path.Combine(_uploadPath, mediaFolder)); 

                // Construct the final file path
                var filePath = Path.Combine(_uploadPath, mediaFolder, originalFileName);

                int counter = 1;
                // Check if the file already exists and create a unique filename if it does
                while (File.Exists(filePath))
                {
                    var newFileName = $"{fileNameWithoutExtension}_{counter}{fileExtension}";
                    filePath = Path.Combine(_uploadPath, mediaFolder, newFileName);
                    counter++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var absolutePath = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL);
                var mediaPath = Path.Combine(mediaFolder, Path.GetFileName(filePath));
                var fileUrl = Path.Combine(absolutePath, mediaFolder, Path.GetFileName(filePath));

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
                    MediaPath = String.Concat("\\", mediaPath),
                    MediaSourceId = model.MediaSourceId,
                    ActionBy = userId,
                    MediaTypeId = mediaTypeId
                };

                var insMedia = await AddMediaAsync(media);
                if (insMedia != null && insMedia.Id > 0)
                {
                    // If upload media to Facebook
                    if (model.UploadToFacebook)
                        return await UploadMediaToFacebook(media, insMedia.Id, fileUrl);

                    return new UResponseWithID
                    {
                        Status = 1,
                        Id = insMedia.Id,
                        Message = "Media added successfully"
                    };
                }

                return new UResponseWithID { Message = "Something went wrong, cannot upload media right now" };
            }

            return new UResponseWithID { Message = "Something went wrong, cannot upload media right now" };
        }

        public async Task<UResponseWithID> UpdateMediaAsync(MediaUploadDto media)
        {

            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Medias_Ops @ActionId={(int)CrudEnum.Update}, @Id={media.Id}, @MediaId={media.MediaId}, @ActionBy={userId}").ToListAsync();
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

                        var mediaTypeId = GetMediaTypeIdFromExtension(fileExtension);

                        // Determine media type folder name
                        string mediaTypeFolder = Path.Combine(MediaSourceEnum.Conversation.ToString(), Enum.GetName(typeof(MediaTypeEnum), mediaTypeId));

                        // Create folder path: uploads/clientId/senderId/mediaType
                        string mediaFolder = _staticFolderPath;
                        if (client.ClientId > 0)
                            mediaFolder = Path.Combine(mediaFolder, client.ClientId.ToString());

                        if (senderName.SenderId > 0)
                            mediaFolder = Path.Combine(mediaFolder, senderName.SenderId.ToString());

                        mediaFolder = Path.Combine(mediaFolder, mediaTypeFolder);

                        // Ensure directories exist
                        if (!Directory.Exists(Path.Combine(_uploadPath, mediaFolder)))
                            Directory.CreateDirectory(Path.Combine(_uploadPath, mediaFolder));

                        // Construct the final file path
                        var filePath = Path.Combine(_uploadPath, mediaFolder, fileName);

                        var mediaPath = Path.Combine(mediaFolder, Path.GetFileName(filePath));

                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await mediaResponse.Content.CopyToAsync(fileStream);
                        }

                        var media = await AddMediaAsync(new MediaUploadDto
                        {
                            ClientId = client.ClientId,
                            SenderNameId = senderName.SenderId,
                            ContentType = mediaResult.mime_type,
                            FileExtension = Path.GetExtension(filePath),
                            FileName = Path.GetFileName(filePath),
                            FileSize = mediaResult.file_size,
                            MediaId = mediaId,
                            MediaPath = String.Concat("\\", mediaPath),
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
            string fileExtension = media.FileExtension.ToLower();

            if (_allowedAudioExtensions.Contains(fileExtension))
                type = MessageTypeEnum.AUDIO;
            else if (_allowedDocumentExtensions.Contains(fileExtension))
                type = MessageTypeEnum.DOCUMENT;
            else if (_allowedImageExtensions.Contains(fileExtension))
                type = MessageTypeEnum.IMAGE;
            else if (_allowedVideoExtensions.Contains(fileExtension))
                type = MessageTypeEnum.VIDEO;

            return type;
        }

        /// <summary>
        /// Check allowed media types for message
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool CheckAllowedMediaTypeForMessage(string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            extension = extension.ToLower();

            List<string> allowedMediaExtensions = new List<string>();
            allowedMediaExtensions.AddRange(_allowedAudioExtensions); //Audio types
            allowedMediaExtensions.AddRange(_allowedDocumentExtensions); //Document types
            allowedMediaExtensions.AddRange(_allowedImageExtensions); // Image types
            allowedMediaExtensions.AddRange(_allowedVideoExtensions); // Video types

            return allowedMediaExtensions.Contains(extension.ToLower());
        }

        /// <summary>
        /// Check allowed media types 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool CheckAllowedMediaType(string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            extension = extension.ToLower();

            List<string> allowedMediaExtensions = new List<string>();
            allowedMediaExtensions.AddRange(_allowedDocumentExtensions); //Document types
            allowedMediaExtensions.AddRange(_allowedImageExtensions); // Image types
            allowedMediaExtensions.AddRange(_allowedVideoExtensions); // Video types

            return allowedMediaExtensions.Contains(extension.ToLower());
        }

        public bool CheckAllowedTemplateHeaderType(TemplateHeaderEnum headerType, string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            extension = extension.ToLower();

            if (headerType == TemplateHeaderEnum.IMAGE)
                return _allowedImageExtensions.Contains(extension);
            else if (headerType == TemplateHeaderEnum.VIDEO)
                return _allowedVideoExtensions.Contains(extension);
            else if (headerType == TemplateHeaderEnum.DOCUMENT)
                return _allowedDocumentExtensions.Contains(extension);

            return false;
        }

        public bool CheckAllowedImageType(string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            extension = extension.ToLower();
            return _allowedImageExtensions.Contains(extension.ToLower());
        }

        public bool CheckAllowedVideoType(string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            extension = extension.ToLower();
            return _allowedVideoExtensions.Contains(extension.ToLower());
        }

        public bool CheckAllowedDocumentType(string extension)
        {
            if (String.IsNullOrWhiteSpace(extension)) return false;

            extension = extension.ToLower();
            return _allowedDocumentExtensions.Contains(extension.ToLower());
        }

        public void ExportCatalog(int clientId, int senderId, byte[] byteArray, string localization)
        {
            // Determine media type folder name
            string mediaTypeFolder = String.Empty;

            mediaTypeFolder = Path.Combine(Enum.GetName(MediaSourceEnum.Catalog));

            // Create folder path: uploads/clientId/senderId/mediaType
            string mediaFolder = _staticFolderPath;
            if (clientId > 0)
                mediaFolder = Path.Combine(mediaFolder, clientId.ToString());

            if (senderId > 0)
                mediaFolder = Path.Combine(mediaFolder, senderId.ToString());

            mediaFolder = Path.Combine(mediaFolder, mediaTypeFolder);

            // Ensure directories exist
            if (!Directory.Exists(Path.Combine(_uploadPath, mediaFolder)))
                Directory.CreateDirectory(Path.Combine(_uploadPath, mediaFolder));

            // Construct the final file path
            var filePath = Path.Combine(_uploadPath, mediaFolder, String.Concat(localization, ".xlsx"));

            if (File.Exists(filePath))
                File.Delete(filePath);

            // Write the byte array to an Excel file
            File.WriteAllBytes(filePath, byteArray);
        }

        #endregion
    }
}
