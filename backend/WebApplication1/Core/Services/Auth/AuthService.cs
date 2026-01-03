using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Dtos.Auth;
using WebApplication1.Core.Entities;
using WebApplication1.Core.Interfaces;
using WebApplication1.Core.Repositories;

namespace WebApplication1.Core.Services.Auth
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
    RoleManager<IdentityRole> _roleManager,
    IConfiguration _configuration,
     ILogService _logService,
     IMapper _mapper,
     TokenCreator tokenCreator) : IAuthService
    {

        public async Task<IEnumerable<UserInfoDto>> GetUsersListAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserInfoDto>();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault().ToUpper();

                var newDto = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = userRole
                };
                userDtos.Add(newDto);
            }
            return userDtos;
        }


        public async Task<IEnumerable<string>> GetUsernamesListAsync()
        {
            var userNames = await _userManager.Users.Select(q => q.UserName).ToListAsync();
            return userNames;
        }


        public async Task<UserInfoDto?> GetUserDetailsByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault()?.ToUpper() ?? StaticUserRoles.USER;

            var userDto = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRole
            };

            return userDto;
        }



        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto dto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(dto.UserName);
            if (isExistsUser != null)
            {
                // return BadRequest(new { message = "UserName already exists.", IsSucceed = false });
                return new GeneralServiceResponseDto { Message = "UserName already exists.", IsSucceed = false };
            }

            //if not exists,then store dto info into sql
            var newUser = _mapper.Map<ApplicationUser>(dto);
            var createUserResult = await _userManager.CreateAsync(newUser, dto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User creation failed because:";
                foreach (var error in createUserResult.Errors)
                {
                    errorString = $"{errorString}# {error.Description}";
                }
                return new GeneralServiceResponseDto { Message = "UserName already exists.", IsSucceed = false };
            }

            // add a default USER role to all new users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);
            // create  a GetLogDto 

            await _logService.SaveNewLogAsync(dto.UserName, $"New user registered: {dto.UserName}");
            return new GeneralServiceResponseDto { Message = "UserName created successfully.", IsSucceed = true };
        }



        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            // 1 find user with username
            var user = await _userManager.FindByNameAsync(dto.UserName);
            // 2 this username not exists
            if (user == null)
            {
                // return BadRequest(new { message = "UserName not exists,please register.", IsSucceed = false });
                return new LoginResponseDto { Message = "UserName not exists,please register.", IsSucceed = false };

            }
            // 3. username exists---this user exists, then check password of this user
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordCorrect)
            {
                // return BadRequest(new { message = "Password is incorrect.", IsSucceed = false });
                return new LoginResponseDto { Message = "Password is incorrect.", IsSucceed = false };
            }

            //4 if username exists and password is correct->finally create token and userinfo to front-end
            await _logService.SaveNewLogAsync(dto.UserName, "New login");

            var token = await tokenCreator.GenerateJWTToken(user);
            // GetRolesAsync返回的是Roles表的的Name,不是NormalizedName,但是我的Roles表新建Name都是小写的,所以要注意大小写的问题!!!!!!!
            // 导致的大问题 FirstOrDefault这就是小写的了-那生成token的时候用的就是小写的了--
            // 然后看解析token出来的 也是小写-- 那对于其他的控制器只要是用Authorize的时候,就会因为大小写不匹配而导致授权失败!!!!!! 
            // ⚠️: 这就是一开始手动生成roles表的时候 忘记大小写的问题了!!!!!!!!!! 那个时候根本没注意这个问题,后来发现token解析出来的角色是小写的,然后就去看数据库发现name字段是小写的,然后就知道是这个问题了
            var userRoles = await _userManager.GetRolesAsync(user);      // 因为我应用就是一个用户就一个角色,但是identity获取中间表的role的时候就只有roles的方法--所以我才用下面取出来第一个,就是转换成字符串(列表不能直接转换成单个字符串，需要先取元素，再处理可能为 null 的情况,所以才用first这个方法)
            string userRole = userRoles.FirstOrDefault().ToUpper() ?? StaticUserRoles.USER;
            Console.WriteLine("Current user role,uppercase or lowercase: " + userRole);

            // 这里没用automapper,是因为知道每个字段怎么来的, 测试起来也清楚
            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRole,
            };

            return new LoginResponseDto
            {
                IsSucceed = true,
                Message = "Login successfully",
                Token = token,
                User = userInfo
            };
        }



        public async Task<GeneralServiceResponseDto> UpdateRoleAsync(UpdateRoleDto dto, ClaimsPrincipal User)
        {
            // 1 find user  with username.  拿到目标用户信息,我要改谁的角色
            var user = await _userManager.FindByNameAsync(dto.UserName); // ApplicationUser not username
            // 2 this username not exists,验证目标用户存在
            if (user == null)
            {
                // return BadRequest(new { message = "Invalid UserName.", IsSucceed = false });
                return new GeneralServiceResponseDto { Message = "Invalid UserName.", IsSucceed = false };

            }
            // 3 just the owner and admin can update roles---这个是操作者要修改的用户的 现在现在数据库中存储的身份有哪些
            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault().ToUpper() ?? StaticUserRoles.USER;

            //4. 检查当前登录用户,是不是admin ---User.IsInRole("ADMIN"),结果是true/false
            // 执行角色修改
            //     如果管理员要把某个用户从 USER 改成 MANAGER，就允许。
            //     如果普通用户尝试改角色 → 拒绝
            // 下面这段 非常乱, 一定要分清楚, 操作者, 目标者 (操作者---更新目标者的角色身份信息 !!!!!!!!!!!!!)
            // step1操作者检查: User是操作者, 也就是当前登录的人, 看这个执行修改的人, 先要判断是不是有权限可以 更改身份 查操作者的权限!!

            // double check: make dto.NewRole uppercase
            dto.NewRole = dto.NewRole.ToUpper();
            if (User.IsInRole(StaticUserRoles.ADMIN))
            {
                // step2新角色检查: 判断 这个要更改的 目标者的--新角色限制检查: 限制 ADMIN 只能将用户的角色设置为普通角色（USER 或 MANAGER）。
                if (dto.NewRole == StaticUserRoles.USER || dto.NewRole == StaticUserRoles.MANAGER)
                {
                    // 目标用户检查:是否是 OWNER 或 ADMIN, 看这个操作者--要修改的这个用户--在现在的数据库中存储的角色身份 是不是有较高的权限的身份,是的话,那这个操作者是无法对这个目标者修改身份 的
                    if (userRole == StaticUserRoles.ADMIN || userRole == StaticUserRoles.OWNER)
                    {
                        // YES, 目标用户是高权限, 那操作者 无权修改这个 人的身份
                        // return BadRequest(new { message = "You are not alllowed to change role of this user.", IsSucceed = false });
                        return new GeneralServiceResponseDto { Message = "You are not alllowed to change role of this user.", IsSucceed = false };
                    }
                    else
                    {
                        // 那目标用户是比操作者低权限的角色--那 操作者可以修改这个人的身份
                        // 1. 移除所有旧角色： 将目标用户（user）当前拥有的所有角色全部删除--但是我应用就是一个用户一个角色,所以用RemoveFromRole 不用RemoveFromRoles
                        await _userManager.RemoveFromRoleAsync(user, userRole);
                        await _userManager.AddToRoleAsync(user, dto.NewRole);
                        // create  a log

                        await _logService.SaveNewLogAsync(user.UserName, "User role updated.");
                        // return Ok(new { message = "Role updated successfully.", IsSucceed = true });
                        return new GeneralServiceResponseDto { Message = "Role updated successfully.", IsSucceed = true };
                    }
                }
                // return BadRequest(new { message = "You are not alllowed to change role of this user.", IsSucceed = false });
                return new GeneralServiceResponseDto { Message = "You are not alllowed to change role of this user.", IsSucceed = false };
            }
            else
            {
                // 如果操作者是 owner(最高权限)
                // 那新角色检查: 只要不是修改的用户是owner这个角色, 那这个操作者都可以修改
                if (userRole == StaticUserRoles.OWNER)
                {
                    // return BadRequest(new GeneralServiceResponseDto { message = "You are not alllowed to change role of this user.", IsSucceed = false });
                    return new GeneralServiceResponseDto { Message = "You are not alllowed to change role of this user.", IsSucceed = false };
                }
                else
                {
                    // 目标者不是owner, 那我这个操作者就是随意的修改了
                    await _userManager.RemoveFromRoleAsync(user, userRole);
                    await _userManager.AddToRoleAsync(user, dto.NewRole);
                    // create  a log
                    await _logService.SaveNewLogAsync(user.UserName, "User role updated.");
                    // return Ok(new { message = "Role updated successfully.", IsSucceed = true });
                    return new GeneralServiceResponseDto { Message = "Role updated successfully.", IsSucceed = true };
                }
            }
        }



    }
}