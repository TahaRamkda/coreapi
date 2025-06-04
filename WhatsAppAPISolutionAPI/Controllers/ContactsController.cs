using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Contact;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        #region Fields
        private readonly int clientId;
        private readonly int userId;
        private readonly IContactService _contactService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ContactsController> _logger;
        private readonly IUserService _userService;
        #endregion

        #region Ctor
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
            userId = _userService.GetUserIdFromAccessToken();
        }
        #endregion

        #region Methods
        [HttpGet("getcontactslist")]
        public async Task<ActionResult> GetContactsListAsync(int GroupId = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogDebug("Calling api GetContactsListAsync with clientId={clientId}, GroupId={GroupId}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, GroupId, SearchStr, SortBy, PageNo, PageSize);

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
            _logger.LogDebug("Calling api GetContactByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            if (id <= 0)
                return Ok(new ApiResult { Message = "Please enter contact id" });

            var res = await _contactService.GetContactByIdAsync(clientId, id);

            _logger.LogDebug("Received api GetContactByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

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
            _logger.LogDebug("Calling api AddContactAsync with request={requst}", JsonConvert.SerializeObject(contact));

            if (contact == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            var response = await _contactService.AddContactAsync(clientId, userId, contact);

            _logger.LogDebug("Received api AddContactAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogDebug("Calling api UpdateContactAsync with request={requst}", JsonConvert.SerializeObject(contact));

            if (contact == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            var response = await _contactService.UpdateContactAsync(clientId, contact);

            _logger.LogDebug("Received api UpdateContactAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogDebug("Calling api DeleteContactAsync with ContactId={ContactId}", ContactId);

            if (ContactId <= 0)
                return NotFound("not found");

            var response = await _contactService.DeleteContactAsync(ContactId);

            _logger.LogDebug("Received api DeleteContactAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogDebug("Calling api ImportContactsAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (model.File == null || model.File.Length <= 0)
                return Ok(new ApiResult { Message = "No file found" });

            var response = await _contactService.ImportBulkContacts(clientId, userId, model);

            _logger.LogDebug("Received api ImportContactsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }
        #endregion
    }
}
