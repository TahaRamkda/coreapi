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
    //[Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService,
            WhatsAppSolutionContext dbContext,
            ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getcontactslist")]
        public async Task<ActionResult> GetContactsListAsync()
        {
            var res = await _contactService.GetContactListAsync();
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getcontactbyid")]
        public ActionResult GetContactByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Contacts.Where(x => x.ContactId == id).FirstOrDefault();

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

        [HttpPost("addContact")]
        public async Task<IActionResult> AddContactAsync([FromBody] ContactDto contact)
        {
            if (contact == null)
            {
                return BadRequest();
            }

            var response = await _contactService.AddContactAsync(contact);
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

        [HttpPost("addbulkcontacts")]
        public async Task<IActionResult> AddBulkContactAsync([FromBody] BulkContactDto contact)
        {
            if (contact == null)
            {
                return BadRequest();
            }

            var response = await _contactService.AddBulkContactAsync(contact);
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

        [HttpPut("updatecontact")]
        public async Task<IActionResult> UpdateContactAsync(ContactDto contact)
        {
            if (contact == null)
            {
                return BadRequest();
            }

            var response = await _contactService.UpdateContactAsync(contact);
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

        [HttpDelete("deletecontact")]
        public async Task<IActionResult> DeleteContactAsync(int contact_Id)
        {
            if (contact_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _contactService.DeleteContactAsync(contact_Id);
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
