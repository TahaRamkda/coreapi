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
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientService clientService,
            WhatsAppSolutionContext dbContext,
            ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getclientslist")]
        public async Task<ActionResult> GetClientsListAsync()
        {
            var res = await _clientService.GetClientListAsync();
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getclientbyid")]
        public ActionResult GetClientByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Clients.Where(x => x.ClientId == id).FirstOrDefault();

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

        [HttpPost("addClient")]
        public async Task<IActionResult> AddClientAsync([FromBody] ClientDto client)
        {
            if (client == null)
            {
                return BadRequest();
            }

            var response = await _clientService.AddClientAsync(client);
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

        [HttpPut("updateclient")]
        public async Task<IActionResult> UpdateClientAsync(ClientDto client)
        {
            if (client == null)
            {
                return BadRequest();
            }

            var response = await _clientService.UpdateClientAsync(client);
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

        [HttpDelete("deleteclient")]
        public async Task<IActionResult> DeleteClientAsync(int client_Id)
        {
            if (client_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _clientService.DeleteClientAsync(client_Id);
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
        [HttpGet("getclientaccesstoken")]
        public ActionResult GetClientAccessTokenAsync(int client_Id)
        {
            if (client_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Clients.Where(x => x.ClientId == client_Id && x.RecordStatus != -1).FirstOrDefault();

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
                Result = response.AccessToken,
                Message = "Data fetch successfully"
            });
        }

    }
}
