#region User Directives
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
#endregion

namespace WhatsAppAPISolutionAPI.Controllers
{
    #region Route
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    #endregion
    public class ItemDetailsController : Controller
    {
        #region Fields
        private readonly ILogger<ItemDetailsController> _logger;
        private readonly IITemsCatalogService _itemsCatalogService;
        #endregion

        #region Ctor
        public ItemDetailsController(ILogger<ItemDetailsController> logger, IITemsCatalogService itemsCatalogService)
        {
            _logger = logger;
            _itemsCatalogService = itemsCatalogService;
        }
        #endregion

        #region Methods

        [HttpGet("GetItemDetails")]
        public async Task<ActionResult> GetItemsDetailAsync(int ItemId, int SenderId, int ClientId, string SearchStr="", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var result = await _itemsCatalogService.GetItemDetailsAsync(ItemId, SenderId, ClientId, SearchStr);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Items Listing",
            });

        }
        [HttpGet("GetItemModifierDetails")]
        public async Task<ActionResult> GetItemModifierDetailsAsync(int ItemId, int ClientId, int SenderId, String SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var result = await _itemsCatalogService.GetItemModifierDetailsAsync(ItemId, ClientId, SenderId, SearchStr, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Items Modifier Listing",
            });
        }
        [HttpGet("GetModifierItemDetails")]
        public async Task<ActionResult> GetModifierItemDetailsAsync(int ItemId, int ClientId, int SenderId, String SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var result = await _itemsCatalogService.GetModifierItemDetails(ItemId, ClientId, SenderId, SearchStr, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Modifier Items Listing",
            });
        }
        #endregion
    }
}
