using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ITemplateService
    {
        public Task<List<UTemplate>> GetTemplateListAsync();
        public Task<UResponse> AddTemplateAsync(TemplateDto template);
        public Task<UResponse> UpdateTemplateAsync(TemplateDto template);
        public Task<UResponse> DeleteTemplateAsync(int templates_Id);
        public Task<UResponse> AddTemplateWithParameterAsync(TemplateWithParametersDto templateWithParameter);
    }
}
