#region Using Directives
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.ITems;
#endregion

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ITemsCatalogService : IITemsCatalogService
    {
        #region Fields
        private readonly WhatsAppSolutionContext2 _dbContext2;
        #endregion

        #region Ctor
        public ITemsCatalogService(WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }
        #endregion

        #region Methods
        public async Task<List<UItems>> GetItemListAsync(int SenderId, int ClientId, string  SearchStr= "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.ITems.FromSqlInterpolated($"exec usp_Catalog_ops @ActionId={(int)ItemsTypeEnum.GetItemsDetails}, @SenderId = {SenderId}, @ClientId={ClientId}, @SearchStr={SearchStr}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<List<UItemModiferDetails>> GetItemModifierDetailsAsync(int ItemId, int ClientId, int SenderId, string SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.ITemModifier.FromSqlInterpolated($"exec usp_Catalog_ops @ActionId={(int)ItemsTypeEnum.GetItemModifierDetails}, @ItemId={ItemId},@ClientId={ClientId}, @SenderId={SenderId}, @PageNo={PageNo}, @PageSize={PageSize}, @SearchStr={SearchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UItemModifierItemDetails>> GetModifierItemDetails(int ModifierGroupId, int clientId, int SenderId, string SearchStr = "", int PageNo =0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.ModifierItem.FromSqlInterpolated($"exec usp_Catalog_ops @ActionId={(int)ItemsTypeEnum.GetModifierItemDetails}, @ModifierId={ModifierGroupId}, @ClientId={clientId}, @SenderId={SenderId}, @PageNo={PageNo}, @PageSize={PageSize}, @SearchStr={SearchStr}").ToListAsync();
            return response;
        }
        #endregion
    }
}
