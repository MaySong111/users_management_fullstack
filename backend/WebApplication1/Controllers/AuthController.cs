using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Dtos.Auth;

using WebApplication1.Core.Services.Auth;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        // public async Task<IActionResult> Register(RegisterDto dto) 可以,但是下面这样统一响应结构,swagger也能看到是返回什么类型
        public async Task<ActionResult<GeneralServiceResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null) return Unauthorized(new { message = "Your credentials are invalid. Please contact to Admin." });
            return Ok(result);
        }

        [HttpPost("update-role")]
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        // 说ClaimsPrincipal User我是操作者, 如我是it管理员,我现在要更改某个员工的权限了--那我到底要改谁的, 改成什么样(这是UpdateRoleDto dto里的信息-操作者要做什么)
        // 要改谁？dto.UserName (这是你想要操作的目标用户的用户名)改成什么样？dto.NewRole (这是你想要设置的新角色)--所以这就是为什么UpdateRoleDto 有这两个属性
        public async Task<ActionResult<GeneralServiceResponseDto>> UpdateRole([FromBody] UpdateRoleDto dto)
        {
            var result = await _authService.UpdateRoleAsync(dto, User);
            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsersListAsync()
        {
            var result = await _authService.GetUsersListAsync();
            return Ok(result);
        }


        // get list of all usernames for send message
        [HttpGet("usernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetUsernamesListAsync()
        {
            var usernames = await _authService.GetUsernamesListAsync();
            return Ok(usernames);
        }


        // get a User by UserName
        [HttpGet("users/{userName}")]
        public async Task<ActionResult<UserInfoDto?>> GetUserDetailsByUserNameAsync([FromRoute] string userName)
        {
            var user = await _authService.GetUserDetailsByUserNameAsync(userName);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            return Ok(user);
        }
    }
}