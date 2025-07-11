#region User Directives
using WhatsAppAPISolutionDL.UserModels.ITems;
#endregion

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IITemsCatalogService
    {
        #region Abstract Methods
        Task<List<UItems>> GetItemListAsync(int SenderId, int ClientId, String SearchStr="", int PageNo = 0, int PageSize = int.MaxValue);
        Task<List<UItemModiferDetails>> GetItemModifierDetailsAsync(int ItemId, int ClientId, int SenderId, string SearchStr = "", int PageNo = 0, int PageSize=int.MaxValue);
        Task<List<UItemModifierItemDetails>> GetModifierItemDetails(int ModifierGroupId, int ClientId, int SenderId, string SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue);
        #endregion 

    }
}
