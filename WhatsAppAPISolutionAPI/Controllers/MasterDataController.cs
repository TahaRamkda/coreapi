using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Master;

[ApiController]
[Route("[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly IMasterDataService _masterDataService;
    private readonly ILogger<MasterDataController> _logger;

    public MasterDataController(IMasterDataService masterDataService, ILogger<MasterDataController> logger)
    {
        _masterDataService = masterDataService;
        _logger = logger;
    }

    [HttpGet("getmasterdatalist")]
    public async Task<ActionResult> GetMasterDataListAsync(string type)
    {
        _logger.LogDebug("Calling api GetMasterDataListAsync with type={type}", type);

        var res = await _masterDataService.GetMasterDataListAsync(type);
        return Ok(new ApiResult
        {
            Success = true,
            Result = res,
            Message = "Data fetched successfully"
        });
    }

    [HttpPost("addmasterdata")]
    public async Task<IActionResult> AddMasterDataAsync([FromBody] MasterDto model)
    {
        var response = await _masterDataService.AddMasterDataAsync(model);
        return Ok(new ApiResult
        {
            Success = response?.Status == 1,
            Message = response?.Message ?? "Failed to add master data",
            Result = response
        });
    }

    [HttpPut("updatemasterdata")]
    public async Task<IActionResult> UpdateMasterDataAsync([FromBody] MasterDto model)
    {
        var response = await _masterDataService.UpdateMasterDataAsync(model);
        return Ok(new ApiResult
        {
            Success = response?.Status == 1,
            Message = response?.Message ?? "Failed to update master data",
            Result = response
        });
    }

    [HttpDelete("deletemasterdata/{mastId}")]
    public async Task<IActionResult> DeleteMasterDataAsync(int mastId)
    {
        var response = await _masterDataService.DeleteMasterDataAsync(mastId);
        return Ok(new ApiResult
        {
            Success = response?.Status == 1,
            Message = response?.Message ?? "Failed to delete master data",
            Result = response
        });
    }

}
