using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly int userId;
        private readonly IClientService _clientService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ClientsController> _logger;
        private readonly IUserService _userService;

        public ClientsController(IClientService clientService,
            WhatsAppSolutionContext dbContext,
            ILogger<ClientsController> logger,
            IUserService userService)
        {
            _clientService = clientService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;


            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("getclientslist")]
        public async Task<ActionResult> GetClientsListAsync(string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetClientsListAsync with searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", SearchStr, SortBy, PageNo, PageSize);

            var res = await _clientService.GetClientListAsync(SearchStr, SortBy, PageNo, PageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getclientbyid")]
        public async Task<ActionResult> GetClientByIdAsync(int id)
        {
            _logger.LogInformation("Calling api GetClientByIdAsync with id={id}", id);

            if (id <= 0)
            {
                return NotFound("not found");
            }

            var res = await _clientService.GetClientByIdAsync(id);

            _logger.LogInformation("Received api GetClientByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addClient")]
        public async Task<IActionResult> AddClientAsync([FromBody] ClientDto client)
        {
            _logger.LogInformation("Calling api AddClientAsync with request={requst}", JsonConvert.SerializeObject(client));

            if (client == null)
            {
                return BadRequest();
            }

            var response = await _clientService.AddClientAsync(userId, client);

            _logger.LogInformation("Received api AddClientAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
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

        [HttpPut("updateclient")]
        public async Task<IActionResult> UpdateClientAsync(ClientDto client)
        {
            _logger.LogInformation("Calling api UpdateClientAsync with request={requst}", JsonConvert.SerializeObject(client));

            if (client == null)
            {
                return BadRequest();
            }

            var response = await _clientService.UpdateClientAsync(userId, client);

            _logger.LogInformation("Received api UpdateClientAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
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
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deleteclient")]
        public async Task<IActionResult> DeleteClientAsync(int ClientId)
        {
            _logger.LogInformation("Calling api DeleteClientAsync with ClientId={ClientId}", ClientId);

            if (ClientId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _clientService.DeleteClientAsync(ClientId);

            _logger.LogInformation("Received api DeleteClientAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
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
                Message = "Data deleted successfully"
            });
        }

        [AllowAnonymous]
        [HttpGet("getclientinformation")]
        public ActionResult GetClientInformationAsync(int ClientId)
        {
            _logger.LogInformation("Calling api GetClientInformationAsync with ClientId={ClientId}", ClientId);

            if (ClientId <= 0)
            {
                return NotFound("not found");
            }

            var response = (from a in _dbContext.Clients
                            join b in _dbContext.SenderNames on a.ClientId equals b.ClientId
                            where a.ClientId == ClientId && a.RecordStatus != -1 && b.RecordStatus != -1
                            select new
                            {
                                ClientId = a.ClientId,
                                ClientName = a.ClientName,
                                AccessToken = a.AccessToken,
                                AppId = a.AppId,
                                BusinessId = a.BusinessId
                            }).FirstOrDefault();

            _logger.LogInformation("Received api GetClientInformationAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
            {
                return Ok(new ApiResult
                {
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getclients")]
        public async Task<IActionResult> GetClientsAsync(int clientId, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetClientsAsync with clientId={clientId}, searchStr={searchStr}", clientId, searchStr);

            var clients = await _clientService.GetClientsAsync(clientId, searchStr);

            //_logger.LogInformation("Received api GetClientsAsync response with data={data}", JsonConvert.SerializeObject(clients));

            if (clients == null || !clients.Any())
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = null,
                    Message = "No records found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = clients,
                Message = String.Empty
            });
        }
    }
}
