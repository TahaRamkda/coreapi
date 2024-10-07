using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ContactService : IContactService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public ContactService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UContact>> GetContactListAsync()
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.Contacts.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddContactAsync(ContactDto contact)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @Group_Id={1}, @First_Name='{2}', @Last_Name='{3}', @Phone_Number='{4}', @Email_Address='{5}', @Area_Name='{6}', @Action_By={7}", (int)CrudEnum.Add, contact.Group_Id, contact.First_Name, contact.Last_Name, contact.Phone_Number, contact.Email_Address, contact.Area_Name, contact.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateContactAsync(ContactDto contact)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @Contact_Id={1}, @Group_Id={2}, @First_Name='{3}', @Last_Name='{4}', @Phone_Number='{5}', @Email_Address='{6}', @Area_Name='{7}', @Action_By={8}", (int)CrudEnum.Update, contact.Contact_Id, contact.Group_Id, contact.First_Name, contact.Last_Name, contact.Phone_Number, contact.Email_Address, contact.Area_Name, contact.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteContactAsync(int contact_Id)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @Contact_Id={1}", (int)CrudEnum.Delete, contact_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
