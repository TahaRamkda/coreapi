using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionAPI.Security;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public TokenController(IConfiguration config, WhatsAppSolutionContext context, TokenService tokenService, WhatsAppSolutionContext dbContext, IUserService userService)
        {
            _configuration = config;
            _context = context;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _userService = userService;
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(ApiUser _userData)
        //{
        //    if (_userData != null && _userData.UserName != null && _userData.Password != null)
        //    {
        //        var user = await GetUser(_userData.UserName, _userData.Password);

        //        if (user != null)
        //        {
        //            //create claims details based on the user information
        //            var claims = new[] {
        //                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
        //                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //                new Claim("UserId", user.ApiUserId.ToString()),
        //                new Claim("DisplayName", user.UserName),
        //                new Claim("UserName", user.UserName),
        //                new Claim("Email", user.UserName)
        //            };

        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //            var token = new JwtSecurityToken(
        //                _configuration["Jwt:Issuer"],
        //                _configuration["Jwt:Audience"],
        //                claims,
        //                expires: DateTime.UtcNow.AddMinutes(10),
        //                signingCredentials: signIn);

        //            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        //        }
        //        else
        //        {
        //            return BadRequest("Invalid credentials");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        //private async Task<ApiUser> GetUser(string email, string password)
        //{
        //    return await _context.ApiUsers.FirstOrDefaultAsync(u => u.UserName == email && u.Password == password);
        //}

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(string accessTokenData, string refreshTokenData)
        {
            if (String.IsNullOrEmpty(accessTokenData) || String.IsNullOrEmpty(refreshTokenData))
                return BadRequest("Invalid client request");

            string accessToken = accessTokenData;
            string refreshToken = refreshTokenData;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpiryTimeInMinutes"]));

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
                User_Id = exist.UserId,
                Client_Id = exist.ClientId,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = refreshTokenExpiryTime
            };

            await _userService.AddUserTokenAsync(user);

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
        public async Task<IActionResult> Revoke(int userId)
        {
            var data = _dbContext.Users.Where(x => x.UserId == userId && x.RecordStatus != -1).FirstOrDefault();
            if (data == null)
                return BadRequest();

            var user = new UserDto
            {
                User_Id = userId,
                Client_Id = data.ClientId,
                AccessToken = "",
                RefreshToken = "",
                RefreshTokenExpiry = DateTime.Now
            };

            var users = await _userService.AddUserTokenAsync(user);
            return NoContent();
        }
    }
}