using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TemplateParameterController : ControllerBase
    {
        private readonly ITemplateParameterService _templateParameterService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<TemplateParameterController> _logger;

        public TemplateParameterController(ITemplateParameterService templateParameterService,
            WhatsAppSolutionContext dbContext,
            ILogger<TemplateParameterController> logger)
        {
            _templateParameterService = templateParameterService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("gettemplateParameterslist")]
        public async Task<ActionResult> GetTemplateParametersListAsync()
        {
            var res = await _templateParameterService.GetTemplateParameterListAsync();
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplateParameterbyid")]
        public ActionResult GetTemplateParameterByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.TemplateParameters.Where(x => x.ParamId == id).FirstOrDefault();

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

        [HttpPost("addTemplateParameter")]
        public async Task<IActionResult> AddTemplateParameterAsync([FromBody] TemplateParameterDto templateParameter)
        {
            if (templateParameter == null)
            {
                return BadRequest();
            }

            var response = await _templateParameterService.AddTemplateParameterAsync(templateParameter);
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

        [HttpPut("updatetemplateParameter")]
        public async Task<IActionResult> UpdateTemplateParameterAsync(TemplateParameterDto templateParameter)
        {
            if (templateParameter == null)
            {
                return BadRequest();
            }

            var response = await _templateParameterService.UpdateTemplateParameterAsync(templateParameter);
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

        [HttpDelete("deletetemplateParameter")]
        public async Task<IActionResult> DeleteTemplateParameterAsync(int templateParameter_Id)
        {
            if (templateParameter_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _templateParameterService.DeleteTemplateParameterAsync(templateParameter_Id);
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
    }
}
