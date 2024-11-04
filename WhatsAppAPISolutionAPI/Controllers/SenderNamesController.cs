using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System.Linq;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class SenderNamesController : ControllerBase
    {
        private readonly ISenderNameService _senderNameService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<SenderNamesController> _logger;

        public SenderNamesController(ISenderNameService senderNameService,
            WhatsAppSolutionContext dbContext,
            ILogger<SenderNamesController> logger)
        {
            _senderNameService = senderNameService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getsenderNameslist")]
        public async Task<ActionResult> GetSenderNamesListAsync(int client_Id)
        {
            var res = await _senderNameService.GetSenderNameListAsync(client_Id);
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getsenderNamebyid")]
        public ActionResult GetSenderNameByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.SenderNames.Where(x => x.SenderId == id).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addSenderName")]
        public async Task<IActionResult> AddSenderNameAsync([FromBody] SenderNameDto senderName)
        {
            if (senderName == null)
            {
                return BadRequest();
            }

            var response = await _senderNameService.AddSenderNameAsync(senderName);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updatesenderName")]
        public async Task<IActionResult> UpdateSenderNameAsync(SenderNameDto senderName)
        {
            if (senderName == null)
            {
                return BadRequest();
            }

            var response = await _senderNameService.UpdateSenderNameAsync(senderName);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletesenderName")]
        public async Task<IActionResult> DeleteSenderNameAsync(int senderName_Id)
        {
            if (senderName_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _senderNameService.DeleteSenderNameAsync(senderName_Id);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [AllowAnonymous]
        [HttpGet("getsendernameinformation")]
        public ActionResult GetSenderNameInformationAsync(int client_Id, int sender_Name_Id)
        {
            if (client_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = (from a in _dbContext.SenderNames
                            join b in _dbContext.Clients on (long)a.ClientId equals b.ClientId
                            where a.ClientId == client_Id && a.SenderId == sender_Name_Id && a.RecordStatus != -1 && b.RecordStatus != -1
                            select new
                            {
                                ClientId = a.ClientId,
                                SenderId = a.SenderId,
                                SenderName = a.SenderName1,
                                PhoneNumberId = a.PhoneNumberId,
                                PhoneNumber = a.PhoneNumber,
                                BusinessAccountId = a.BusinessAccountId,
                                AccessToken = b.AccessToken,
                                AppId = b.AppId,
                                BusinessId = b.BusinessId
                            }).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }
    }
}
