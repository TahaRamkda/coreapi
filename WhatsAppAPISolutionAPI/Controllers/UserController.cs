using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WhatsAppAPISolutionAPI.Security;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

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
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userservice,
            WhatsAppSolutionContext dbContext,
            TokenService tokenService,
            IConfiguration configuration,
            ILogger<UserController> logger)
        {
            _userService = userservice;
            _dbContext = dbContext;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<ActionResult> Login(string Username, string Password)
        {
            _logger.LogDebug("Calling api Login with Username={Username}, Password={Password}", Username, Password);

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            { 
                var res = await _userService.Login(Username.Trim(), Password.Trim());

                _logger.LogDebug("Received api Login response with data={data}", JsonConvert.SerializeObject(res));

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
                    //new Claim("UserId", CommonHelper.Base64Encode(String.Concat(CommonHelper.GenerateRandomKey(), "M", res.UserId, "O",CommonHelper.GenerateRandomKey()))),
                    //new Claim("ClientId", CommonHelper.Base64Encode(String.Concat(CommonHelper.GenerateRandomKey(), "M", res.ClientId, "O",CommonHelper.GenerateRandomKey()))),
                    new Claim("UserId", res.UserId.ToString()),
                    new Claim("ClientId", res.ClientId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                //take 4 letter alphanumeric _ id _ take 4 letter alphanumeric

                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpiryTimeInMinutes"]));
                var user = new UserDto
                {
                    UserId = res.UserId,
                    ClientId = res.ClientId,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiryTime
                };

                var response = await _userService.AddUserTokenAsync(user);

                _logger.LogDebug("Received function AddUserTokenAsync response with data={data}", JsonConvert.SerializeObject(response));

                res.AccessToken = accessToken;
                res.RefreshToken = refreshToken;
                res.RefreshTokenExpiry = refreshTokenExpiryTime;

                return Ok(new ApiResult
                {
                    Success = true,
                    Result = res,
                    Message = ""
                });
            }

            return Ok(new ApiResult
            {
                Message = "Username and password required",
            });
        }

        [HttpGet("getuserbyid")]
        public async Task<ActionResult> GetUserByIdAsync(int clientId, int id)
        {
            _logger.LogDebug("Calling api GetUserByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (id <= 0)
                return Ok(new ApiResult { Message = "not found" });

            var user = await _userService.GetUserByIdAsync(clientId, id);

            _logger.LogDebug("Received api GetUserByIdAsync response with data={data}", JsonConvert.SerializeObject(user));

            if (user == null)
            {
                return Ok(new ApiResult
                {
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = user,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDto user)
        {
            _logger.LogDebug("Calling api AddUserAsync with request={requst}", JsonConvert.SerializeObject(user));

            if (user == null)
            {
                return BadRequest();
            }

            var response = await _userService.AddUserAsync(user);

            _logger.LogDebug("Received api AddUserAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {

                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "User added successfully"
            });
        }

        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUserAsync(UserDto user)
        {
            _logger.LogDebug("Calling api UpdateUserAsync with request={requst}", JsonConvert.SerializeObject(user));

            if (user == null)
            {
                return BadRequest();
            }

            var response = await _userService.UpdateUserAsync(user);

            _logger.LogDebug("Received api UpdateUserAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {

                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "User updated successfully"
            });
        }

        [HttpDelete("deleteuser")]
        public async Task<IActionResult> DeleteUserAsync(int UserId, int ClientId)
        {
            _logger.LogDebug("Calling api DeleteUserAsync with UserId={UserId}, ClientId={ClientId}", UserId, ClientId);

            if (UserId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _userService.DeleteUserAsync(UserId, ClientId);

            _logger.LogDebug("Received api DeleteUserAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {

                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "User deleted successfully"
            });
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync(UserDto user)
        {
            _logger.LogDebug("Calling api ChangePasswordAsync with request={requst}", JsonConvert.SerializeObject(user));

            if (user == null)
            {
                return BadRequest();
            }

            var response = await _userService.ChangePasswordAsync(user);

            _logger.LogDebug("Received api ChangePasswordAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {

                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Password Changed successfully"
            });
        }

        [HttpGet("getuserslist")]
        public async Task<ActionResult> GetUsersListAsync(int clientId, string searchStr = "")
        {
            _logger.LogDebug("Calling api GetUsersListAsync with clientId={clientId}, searchStr={searchStr}", clientId, searchStr);

            try
            {
                var res = await _userService.GetUsersListAsync(clientId, searchStr);
                return Ok(new ApiResult
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


        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<ActionResult> token([FromBody] TokenDto model)
        {
            _logger.LogDebug("Calling api Login with Username={Username}, Password={Password}", model.username, model.password);

            if (!string.IsNullOrEmpty(model.username) && !string.IsNullOrEmpty(model.password))
            {
                //var res = await _userService.Login(username.Trim(), password.Trim());
                var res = await _userService.Login(model.username.Trim(), model.password.Trim(), (int)MasterRoleTypeEnum.APIUser);
                if (res == null || res.Status <= 0)
                _logger.LogDebug("Received api token response with data={data}", JsonConvert.SerializeObject(res));

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
                    //new Claim("UserId", CommonHelper.Base64Encode(String.Concat(CommonHelper.GenerateRandomKey(), "M", res.UserId, "O",CommonHelper.GenerateRandomKey()))),
                    //new Claim("ClientId", CommonHelper.Base64Encode(String.Concat(CommonHelper.GenerateRandomKey(), "M", res.ClientId, "O",CommonHelper.GenerateRandomKey()))),
                    new Claim("UserId", res.UserId.ToString()),
                    new Claim("ClientId", res.ClientId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                //take 4 letter alphanumeric _ id _ take 4 letter alphanumeric

                var accessToken = _tokenService.GenerateAccessToken(claims);
                var user = new UserDto
                {
                    AccessToken = accessToken,
                };
                var response = await _userService.AddUserTokenAsync(user);
                _logger.LogDebug("Received function AddUserTokenAsync response with data={data}", JsonConvert.SerializeObject(response));
                res.AccessToken = accessToken;
                return Ok(new ApiResult
                {
                    Success = true,
                    Result = res.AccessToken,
                    Message = ""
                });
            }
            return Ok(new ApiResult
            {
                Message = "Username and password required",
            });
        }
    }
}
