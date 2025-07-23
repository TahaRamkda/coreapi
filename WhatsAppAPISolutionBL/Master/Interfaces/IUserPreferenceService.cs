using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Bridge;
using WhatsAppAPISolutionDL.Dto.Common;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IUserPreferenceService
    {
        Task<ApiResult> Processuserpreference(UserPreferenceDto Dto);
    }
}
