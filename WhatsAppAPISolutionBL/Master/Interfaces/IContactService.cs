using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IContactService
    {
        public Task<List<UContact>> GetContactListAsync(int ClientId, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        public Task<UResponse> AddContactAsync(ContactDto contact);
        public Task<UResponse> AddBulkContactAsync(BulkContactDto contact);
        public Task<UResponse> UpdateContactAsync(ContactDto contact);
        public Task<UResponse> DeleteContactAsync(int ContactId);
    }
}
