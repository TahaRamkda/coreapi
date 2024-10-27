using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Helper;
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

        public async Task<List<UContact>> GetContactListAsync(int client_Id, string searchStr = "")
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @Client_Id={1}, @SearchStr={2}", (int)CrudEnum.List, client_Id, searchStr);
            var response = await _dbContext2.Contacts.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddContactAsync(ContactDto contact)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @Group_Id={1}, @First_Name='{2}', @Last_Name='{3}', @Phone_Number='{4}', @Email_Address='{5}', @Area_Name='{6}', @Action_By={7}", (int)CrudEnum.Add, contact.Group_Id, contact.First_Name, contact.Last_Name, contact.Phone_Number, contact.Email_Address, contact.Area_Name, contact.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> AddBulkContactAsync(BulkContactDto contact)
        {
            if (contact == null) throw new ArgumentNullException(nameof(contact));

            if (contact.Group_Id == 0)
                return new UResponse()
                {
                    Status = 0,
                    Message = "Please insert group"
                };
            if (!contact.ContactsInfo.Any())
                return new UResponse()
                {
                    Status = 0,
                    Message = "Please add atleast one contact"
                };
            if (!contact.ContactsInfo.Select(x => x.Phone_Number).Any())
                return new UResponse()
                {
                    Status = 0,
                    Message = "Please add phone numbers"
                };

            //foreach (var phoneNumber in contact.PhoneNumbers.Select(num => num.Replace("+", "")).ToList())
            //{
            //    var contactInfo = new ContactDto()
            //    {
            //        Group_Id = contact.Group_Id,
            //        Phone_Number = phoneNumber,
            //        ActionBy = 1
            //    };
            //    await AddContactAsync(contactInfo);
            //}

            contact.ContactsInfo = contact.ContactsInfo
    .Where(x => !string.IsNullOrWhiteSpace(x.Phone_Number))
    .Select(x => new ContactInfo
    {
        First_Name = x.First_Name,
        Last_Name = x.Last_Name,
        Phone_Number = x.Phone_Number.Replace("+", "").Trim(),
        Email_Address = x.Email_Address,
        Area_Name = x.Area_Name
    })
    .ToList();

            var batches = contact.ContactsInfo.ChunkBy(50);

            foreach (var batch in batches)
            {
                foreach (var contactInfo in batch)
                {
                    var newContact = new ContactDto()
                    {
                        First_Name = contactInfo.First_Name,
                        Last_Name = contactInfo.Last_Name,
                        Phone_Number = contactInfo.Phone_Number,
                        Email_Address = contactInfo.Email_Address,
                        Area_Name = contactInfo.Area_Name,
                        Group_Id = contact.Group_Id,
                        ActionBy = contact.ActionBy
                    };

                    await AddContactAsync(newContact);
                }
            }
            return new UResponse()
            {
                Status = 1,
                Message = "Data added successfully"
            };
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
