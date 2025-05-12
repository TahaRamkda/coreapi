using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.External;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;
using WhatsAppAPISolutionDL.UserModels.SenderName;
using WhatsAppAPISolutionDL.UserModels.Template;
using static WhatsAppAPISolutionDL.Dto.Media.MediaUploadBridgeDto;

namespace WhatsAppAPISolutionAPI.Controllers
{
    public class ExternalServiceController : Controller
    {
        private readonly IMediatorService _mediatorService;
        private readonly ILogger _logger;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public ExternalServiceController(
          ILogger<ExternalServiceController> logger,
          IMediatorService mediatorService,
          WhatsAppSolutionContext2 dbContext2
          )
        {
            _mediatorService = mediatorService;
            _logger = logger;
            _dbContext2 = dbContext2;
        }


        [HttpPost("sendretrytemplate")]
        public async Task<IActionResult> SendRetryTemplate([FromBody]SendRetryTemplate _templetedetail)
        {
            _logger.LogInformation("Calling function processdbresponse with data={_dbresponse}", JsonConvert.SerializeObject(_templetedetail));

            var response = await _mediatorService.ProcessDBResponse(_templetedetail.ClientId , _templetedetail.SenderId , _templetedetail.DbResponse);

            _logger.LogInformation("Received function process responsee response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.StatusCode <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPost("ReprocessOrder")]
        public async Task<IActionResult> ReprocessOrder([FromBody] ReprocessOrder _orderdetail)
        {
            _logger.LogInformation("Calling function ProcessOrder with data={_orderdetail}", JsonConvert.SerializeObject(_orderdetail));
            var response = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_Followup @ClientId={_orderdetail.ClientId}, @SenderId={_orderdetail.SenderId}, @PhoneNumber={_orderdetail.PhoneNumber}, @OrderId={_orderdetail.OrderId}").ToListAsync();
            _logger.LogInformation("Received database procedure usp_Orders_Followup   response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
            {
                return Ok(new ApiResult
                {
                    Result = null,
                    Message = "Error in DB procedure"
                });

            }

            var dbresponse = await _mediatorService.ProcessDBResponse(_orderdetail.ClientId, _orderdetail.SenderId, response[0]);
            return Ok(new ApiResult
            {
                Success = true,
                Result = dbresponse,
                Message = "Data added successfully"
            });
        }
    }
}
