using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppAPISolutionAPI.Security;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly WhatsAppSolutionContext _context;
        private readonly TokenService _tokenService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly IUserService _userService;
        private readonly ILogger<TokenController> _logger;

        public TokenController(
            IConfiguration config,
            WhatsAppSolutionContext context,
            TokenService tokenService,
            WhatsAppSolutionContext dbContext,
            IUserService userService,
            ILogger<TokenController> logger)
        {
            _configuration = config;
            _context = context;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(string AccessTokenData, string RefreshTokenData)
        {
            _logger.LogInformation("Calling api Refresh with AccessTokenData={AccessTokenData}, RefreshTokenData={RefreshTokenData}", AccessTokenData, RefreshTokenData);

            if (String.IsNullOrEmpty(AccessTokenData) || String.IsNullOrEmpty(RefreshTokenData))
                return BadRequest("Invalid client request");

            string accessToken = AccessTokenData;
            string refreshToken = RefreshTokenData;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpiryTimeInMinutes"]));

            var exist = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower() && x.RecordStatus != -1);
            if (exist == null)
            {
                return Ok(new ApiResult
                {
                    Message = "Invalid access"
                });
            }

            var user = new UserDto
            {
                UserId = exist.UserId,
                ClientId = exist.ClientId,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = refreshTokenExpiryTime
            };

            var users = await _userService.AddUserTokenAsync(user);

            _logger.LogInformation("Received api Refresh response with data={data}", JsonConvert.SerializeObject(users));

            return Ok(new ApiResult
            {
                Success = true,
                Result = new
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiry = refreshTokenExpiryTime
                },
                Message = "Token generated successfully"
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke(int UserId)
        {
            _logger.LogInformation("Calling api Revoke with UserId={UserId}", UserId);

            var data = _dbContext.Users.Where(x => x.UserId == UserId && x.RecordStatus != -1).FirstOrDefault();
            if (data == null)
                return BadRequest();

            var user = new UserDto
            {
                UserId = UserId,
                ClientId = data.ClientId,
                AccessToken = "",
                RefreshToken = "",
                RefreshTokenExpiry = DateTime.UtcNow
            };

            var users = await _userService.AddUserTokenAsync(user);

            _logger.LogInformation("Received api Revoke response with data={data}", JsonConvert.SerializeObject(users));

            return NoContent();
        }
    }
}