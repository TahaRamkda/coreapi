using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IClientService
    {
        public Task<List<UClient>> GetClientListAsync(string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        public Task<UResponse> AddClientAsync(ClientDto client);
        public Task<UResponse> UpdateClientAsync(ClientDto client);
        public Task<UResponse> DeleteClientAsync(int clientId);
    }
}
