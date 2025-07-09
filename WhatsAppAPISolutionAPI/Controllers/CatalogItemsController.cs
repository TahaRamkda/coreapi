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
    public class CatalogItemsController : Controller
    {
        #region Fields
        private readonly ILogger<CatalogItemsController> _logger;
        private readonly IITemsCatalogService _itemsCatalogService;
        private readonly int clientId;
        private readonly IUserService _userService;
        #endregion

        #region Ctor
        public CatalogItemsController(ILogger<CatalogItemsController> logger, IITemsCatalogService itemsCatalogService, IUserService userService)
        {
            _logger = logger;
            _itemsCatalogService = itemsCatalogService;
            _userService = userService; // ✅ assign it first
            clientId = _userService.GetClientIdFromAccessToken();
        }
        #endregion

        #region Methods

        [HttpGet("GetItemList")]
        public async Task<ActionResult> GetItemsListAsync(int SenderId, string SearchStr="", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var result = await _itemsCatalogService.GetItemListAsync(SenderId, clientId, SearchStr);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Items Listing",
            });

        }
        [HttpGet("GetItemModifierDetails")]
        public async Task<ActionResult> GetItemModifierDetailsAsync(int SenderId, String SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var result = await _itemsCatalogService.GetItemModifierDetailsAsync(clientId, SenderId, SearchStr, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Items Modifier Listing",
            });
        }
        [HttpGet("GetModifierItemDetails")]
        public async Task<ActionResult> GetModifierItemDetailsAsync(int SenderId, String SearchStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            var result = await _itemsCatalogService.GetModifierItemDetails(clientId, SenderId, SearchStr, PageNo, PageSize);
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
