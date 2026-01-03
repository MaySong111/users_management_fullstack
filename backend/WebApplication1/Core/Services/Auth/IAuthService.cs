using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Dtos.Auth;

namespace WebApplication1.Core.Services.Auth
{
    public interface IAuthService
    {
        Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<GeneralServiceResponseDto> UpdateRoleAsync([FromBody] UpdateRoleDto dto, ClaimsPrincipal user);
        Task<IEnumerable<UserInfoDto>> GetUsersListAsync();
        Task<IEnumerable<string>> GetUsernamesListAsync();
        Task<UserInfoDto?> GetUserDetailsByUserNameAsync(string userName);

    }
}