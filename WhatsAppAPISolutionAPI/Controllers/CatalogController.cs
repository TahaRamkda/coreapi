using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Catalog;
using WhatsAppAPISolutionDL.Dto.Common;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly ICatalogService _catalogService;

        public CatalogController(ILogger<CatalogController> logger,
            ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        [HttpPost("Import")]
        public async Task<IActionResult> Import([FromQuery] int ClientId, [FromQuery] int SenderId, CatalogDto model)
        {
            if (ClientId == 0)
                return Ok(new ApiResult { Message = "Client id is required" });

            if (SenderId == 0)
                return Ok(new ApiResult { Message = "Sender id is required" });

            if (model == null || model.menu == null)
                return Ok(new ApiResult { Message = "Menu is required" });

            if (model.menu.items == null || model.menu.items.Count == 0)
                return Ok(new ApiResult { Message = "Items is required" });

            if (model.menu.categories == null || model.menu.categories.Count == 0)
                return Ok(new ApiResult { Message = "Categories is required" });

            if (model.menu.items.Any(x => x.name == null || String.IsNullOrEmpty(x.name.en)))
                return Ok(new ApiResult { Message = "English name is required for all items" });

            if (model.menu.items.Any(x => x.price_info == null || x.price_info.price < 0))
                return Ok(new ApiResult { Message = "Price should be present and should not be negative in items" });

            var permissibleItemTypes = new List<string> { "item", "choice" };
            if (model.menu.items.Any(x => String.IsNullOrWhiteSpace(x.type) || !permissibleItemTypes.Contains(x.type.ToLower())))
                return Ok(new ApiResult { Message = "Permissible values for type items are 'ITEM' or 'CHOICE'" });

            if (model.menu.modifiers.Any(x => x.name == null || String.IsNullOrEmpty(x.name.en)))
                return Ok(new ApiResult { Message = "English name is required for modifiers" });

            // Validate no duplicate button names
            var duplicateCategoryIds = model.menu.categories.GroupBy(item => item.id?.Trim()).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
            if (duplicateCategoryIds.Any())
                return Ok(new ApiResult { Message = "Duplicate category id not allowed." });

            var duplicateItemIds = model.menu.items.GroupBy(item => item.id?.Trim()).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
            if (duplicateItemIds.Any())
                return Ok(new ApiResult { Message = "Duplicate item id not allowed." });

            await _catalogService.ImportCatalog(ClientId, SenderId, model);

            return Ok(new ApiResult { Success = true, StatusCode = 200, Message = "Import successful" });
        }

        [HttpGet("Export")]
        public async Task<IActionResult> Export(int ClientId, int SenderId)
        {
            if (ClientId == 0)
                return Ok(new ApiResult { Message = "Client id is required" });

            if (SenderId == 0)
                return Ok(new ApiResult { Message = "Sender id is required" });

            await _catalogService.ExportCatalog(ClientId, SenderId);

            return Ok();
        }

        [HttpPost("GenerateCatalogFlows")]
        public async Task<IActionResult> GenerateCatalogFlows(List<CatalogFlowGenerationDto> model)
        {
            if (model == null || model.Count == 0)
                return Ok(new ApiResult { Message = "Item ids is required" });

            await _catalogService.GenerateCatalogFlows(model);
            return Ok();
        }
    }
}
