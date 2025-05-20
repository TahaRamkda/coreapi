#region User Directives
using WhatsAppAPISolutionDL.UserModels.ITems;
#endregion

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IITemsCatalogService
    {
        #region Abstract Methods
        Task<List<UItems>> GetItemDetailsAsync(int ItemId, int SenderId, int ClientId, String SearchStr="", int PageNo = 0, int PageSize = int.MaxValue);
        Task<List<UItemModiferDetails>> GetItemModifierDetailsAsync(int itemId, int ClientId, int SenderId, string SearchStr = "", int PageNo = 0, int PageSize=int.MaxValue);
        Task<List<UItemModifierItemDetails>> GetModifierItemDetails(int itemId, int ClientId, int SenderId, string SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue);
        #endregion 

    }
}
