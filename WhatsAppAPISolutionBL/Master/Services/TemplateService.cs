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
    public class TemplateService : ITemplateService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public TemplateService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UTemplate>> GetTemplateListAsync()
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.Templates.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddTemplateAsync(TemplateDto template)
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Client_Id={1}, @Template_Name='{2}', @Integration_Id='{3}', @Template_Id='{4}', @Status={5}, @Template_Type={6}, @Action_By={7}", (int)CrudEnum.Add, template.Client_Id, template.Template_Name, template.Integration_Id, template.Template_Id, template.Status, template.Template_Type, template.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateTemplateAsync(TemplateDto template)
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Templates_Id={1}, @Client_Id={2}, @Template_Name='{3}', @Integration_Id='{4}', @Template_Id='{5}', @Status={6}, @Template_Type={7}, @Action_By={8}", (int)CrudEnum.Update, template.Templates_Id, template.Client_Id, template.Template_Name, template.Integration_Id, template.Template_Id, template.Status, template.Template_Type, template.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteTemplateAsync(int template_Id)
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Templates_Id={1}", (int)CrudEnum.Delete, template_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
