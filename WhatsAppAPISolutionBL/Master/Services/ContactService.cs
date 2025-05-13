using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Contact;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Contact;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ContactService : IContactService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IImportManager _importManager;
        private readonly ICacheService _cacheService;
        public ContactService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IImportManager importManager,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _importManager = importManager;
            _cacheService = cacheService;
        }

        public async Task<List<UContact>> GetContactListAsync(int ClientId, int GroupId = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.Contacts.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @GroupId={GroupId}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddContactAsync(int clientId, int userId, ContactDto contact)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @GroupId={contact.GroupId}, @FirstName={contact.FirstName}, @LastName={contact.LastName}, @PhoneNumber={contact.PhoneNumber}, @EmailAddress={contact.EmailAddress}, @AreaName={contact.AreaName}, @ActionBy={userId}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CONTACT_PATTERN_KEY);
            return response[0];
        }


        public async Task<UResponse> UpdateContactAsync(int userId, ContactDto contact)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Update}, @ContactId={contact.ContactId}, @GroupId={contact.GroupId}, @FirstName={contact.FirstName}, @LastName={contact.LastName}, @PhoneNumber={contact.PhoneNumber}, @EmailAddress={contact.EmailAddress}, @AreaName={contact.AreaName}, @ActionBy={userId}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CONTACT_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponse> DeleteContactAsync(int ContactId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Delete}, @ContactId={ContactId}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CONTACT_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponse> ImportBulkContacts(int clientId, int userId, ImportContactDto model)
        {
            var contacts = _importManager.ImportContactsFromXlsx(model.File.OpenReadStream());
            if (contacts == null || !contacts.Any())
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot import contacts, No contacts found"
                };
            }

            var contactsJson = JsonSerializer.Serialize(contacts);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops  @ActionId={(int)CrudEnum.BulkContact}, @ClientId={clientId}, @BulkContact={contactsJson}, @ActionBy={userId}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CONTACT_PATTERN_KEY);
            return response[0];
        }

        public async Task<UContactDetail> GetContactByIdAsync(int clientId, int contactId)
        {
            var cacheKey = string.Format(CacheKeys.CONTACT_BY_ID_KEY, clientId, contactId);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.ContactDetails.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId},@ContactId={contactId}").ToListAsync();
                if (response == null)
                    return null;
                return response[0];
            });

            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);

            return cacheResult;
        }

    }
}
