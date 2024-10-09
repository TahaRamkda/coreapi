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
    public class TemplateParameterService : ITemplateParameterService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public TemplateParameterService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UTemplateParameter>> GetTemplateParameterListAsync()
        {
            var query = string.Format(@"exec usp_Template_Parameters_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.TemplateParameters.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddTemplateParameterAsync(TemplateParameterDto templateParameter)
        {
            var query = string.Format(@"exec usp_Template_Parameters_Ops @ActionId={0}, @Templates_Id={1}, @Client_Id={2}, @Sequence={3}, @Param_Name='{4}', @Param_Type={5}, @Param_Default_Value='{6}', @Status={7}, @Action_By={8}", (int)CrudEnum.Add, templateParameter.Templates_Id, templateParameter.Client_Id, templateParameter.Sequence, templateParameter.Param_Name, templateParameter.Param_Type, templateParameter.Param_Default_Value, templateParameter.Status, templateParameter.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateTemplateParameterAsync(TemplateParameterDto templateParameter)
        {
            var query = string.Format(@"exec usp_Template_Parameters_Ops @ActionId={0}, @Param_Id={1}, @Templates_Id={2}, @Client_Id={3}, @Sequence={4}, @Param_Name='{5}', @Param_Type={6}, @Param_Default_Value='{7}', @Status={8}, @Action_By={9}", (int)CrudEnum.Update, templateParameter.Param_Id, templateParameter.Templates_Id, templateParameter.Client_Id, templateParameter.Sequence, templateParameter.Param_Name, templateParameter.Param_Type, templateParameter.Param_Default_Value, templateParameter.Status, templateParameter.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteTemplateParameterAsync(int templateParameter_Id)
        {
            var query = string.Format(@"exec usp_Template_Parameters_Ops @ActionId={0}, @Param_Id={1}", (int)CrudEnum.Delete, templateParameter_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
