using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Globalization;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Contact;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Agent;
using static WhatsAppAPISolutionDL.Dto.Message.WhatsAppMessageReceiveDto;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly int clientId;
        private readonly IContactService _contactService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ContactsController> _logger;
        private readonly IUserService _userService;

        public ContactsController(IContactService contactService,
            WhatsAppSolutionContext dbContext,
            ILogger<ContactsController> logger,
            IUserService userService)
        {
            _contactService = contactService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("getcontactslist")]
        public async Task<ActionResult> GetContactsListAsync(int GroupId = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetContactsListAsync with clientId={clientId}, GroupId={GroupId}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, GroupId, SearchStr, SortBy, PageNo, PageSize);

            var res = await _contactService.GetContactListAsync(clientId, GroupId, SearchStr, SortBy, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getcontactbyid")]
        public async Task<ActionResult> GetContactByIdAsync(int id)
        {
            _logger.LogInformation("Calling api GetContactByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            if (id <= 0)
                return Ok(new ApiResult { Message = "Please enter contact id" });

            var res = await _contactService.GetContactByIdAsync(clientId, id);

            _logger.LogInformation("Received api GetContactByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
                return Ok(new ApiResult { Message = "No record found with this id" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addContact")]
        public async Task<IActionResult> AddContactAsync([FromBody] ContactDto contact)
        {
            _logger.LogInformation("Calling api AddContactAsync with request={requst}", JsonConvert.SerializeObject(contact));

            if (contact == null)
                return BadRequest();

            contact.ClientId = clientId;
            if (contact.ClientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            var response = await _contactService.AddContactAsync(contact);

            _logger.LogInformation("Received api AddContactAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updatecontact")]
        public async Task<IActionResult> UpdateContactAsync(ContactDto contact)
        {
            _logger.LogInformation("Calling api UpdateContactAsync with request={requst}", JsonConvert.SerializeObject(contact));

            if (contact == null)
                return BadRequest();

            contact.ClientId = clientId;
            if (contact.ClientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            var response = await _contactService.UpdateContactAsync(contact);

            _logger.LogInformation("Received api UpdateContactAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletecontact")]
        public async Task<IActionResult> DeleteContactAsync(int ContactId)
        {
            _logger.LogInformation("Calling api DeleteContactAsync with ContactId={ContactId}", ContactId);

            if (ContactId <= 0)
                return NotFound("not found");

            var response = await _contactService.DeleteContactAsync(ContactId);

            _logger.LogInformation("Received api DeleteContactAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [HttpPost("importcontacts")]
        public async Task<IActionResult> ImportContactsAsync([FromForm] ImportContactDto model)
        {
            _logger.LogInformation("Calling api ImportContactsAsync with request={requst}", JsonConvert.SerializeObject(model));

            model.ClientId = clientId;
            if (model.ClientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (model.File == null || model.File.Length <= 0)
                return Ok(new ApiResult { Message = "No file found" });

            var response = await _contactService.ImportBulkContacts(model);

            _logger.LogInformation("Received api ImportContactsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }
    }
}
