using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionAPI.Security;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Azure;
using WhatsAppAPISolutionDL.Dto;
using Microsoft.EntityFrameworkCore;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly TokenService _tokenService;

        public UserController(IUserService userservice, WhatsAppSolutionContext dbContext, TokenService tokenService, IConfiguration configuration)
        {
            _userService = userservice;
            _dbContext = dbContext;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public async Task<ActionResult> Login(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var res = await _userService.Login(username.Trim(), password.Trim());
                if (res == null || res.Status <= 0)
                {
                    return Ok(new ApiResult
                    {
                        Message = res?.Message
                    });
                }

                //JWT token and refresh token and save in db
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, res.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenExpiryTime = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpiryTimeInMinutes"]));
                var user = new UserDto
                {
                    User_Id = res.UserId,
                    Client_Id = res.ClientId,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiryTime
                };

                await _userService.AddUserTokenAsync(user);

                res.AccessToken = accessToken;
                res.RefreshToken = refreshToken;
                res.RefreshTokenExpiry = refreshTokenExpiryTime;

                return Ok(new ApiResult()
                {
                    Success = true,
                    Result = res,
                    Message = ""
                });
            }
            return Ok(new ApiResult()
            {
                Success = false,
                Result = "Username and password required",
                Message = ""
            });
        }

        [HttpGet("getuserbyid")]
        public async Task<ActionResult> GetUserByIdAsync(int id)
        {
            var user = _dbContext.Users.Where(x => x.UserId == id && x.RecordStatus != -1).FirstOrDefault();

            if (user == null)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = "",
                    Message = "No record found with this id"
                });
            }
            var roleIds = await _dbContext.UsersRoles.Where(x => x.UserId == user.UserId).Select(x => x.RoleId).ToListAsync();

            return Ok(new ApiResult
            {
                Success = true,
                Result = new UserDto
                {
                    User_Id = user.UserId,
                    Client_Id = user.ClientId,
                    UserName = user.UserName,
                    IsActive = user.IsActive,
                    FullName = user.FullName,
                    UserRoles = roleIds != null ? string.Join(",", roleIds) : string.Empty
                },
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var response = await _userService.AddUserAsync(user);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "User added successfully"
            });
        }

        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUserAsync(UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var response = await _userService.UpdateUserAsync(user);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "User updated successfully"
            });
        }

        [HttpDelete("deleteuser")]
        public async Task<IActionResult> DeleteUserAsync(int user_Id, int client_Id)
        {
            if (user_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _userService.DeleteUserAsync(user_Id, client_Id);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "User deleted successfully"
            });
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync(UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var response = await _userService.ChangePasswordAsync(user);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Password Changed successfully"
            });
        }

        [HttpGet("getuserslist")]
        public async Task<ActionResult> GetUsersListAsync(int clientId)
        {
            try
            {
                var res = await _userService.GetUsersListAsync(clientId);
                return Ok(new ApiResult()
                {
                    Success = true,
                    Result = res,
                    Message = "Data fetch successfully"
                });
            }
            catch
            {
                throw;
            }
        }
    }
}
