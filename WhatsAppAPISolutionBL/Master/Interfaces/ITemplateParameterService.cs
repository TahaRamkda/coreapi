using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ITemplateParameterService
    {
        public Task<List<UTemplateParameter>> GetTemplateParameterListAsync();
        public Task<UResponse> AddTemplateParameterAsync(TemplateParameterDto templateParameter);
        public Task<UResponse> UpdateTemplateParameterAsync(TemplateParameterDto templateParameter);
        public Task<UResponse> DeleteTemplateParameterAsync(int templateParameterId);
    }
}
