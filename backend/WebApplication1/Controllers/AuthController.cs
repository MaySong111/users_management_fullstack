using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Dtos.Auth;
using WebApplication1.Core.Entities;
using WebApplication1.Core.Interfaces;
using WebApplication1.Core.Repositories;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private ILogService logService { get; }
        private IMapper mapper { get; }
        private readonly TokenCreator tokenCreator;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogService logService, IMapper mapper, TokenCreator tokenCreator)
        {
            this.tokenCreator = tokenCreator;
            this.mapper = mapper;
            this.logService = logService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        //我是在一开始想简单: 手动在数据库这identityRole表中增加的4行数据的,并不是用的下面的方法
        // public async Task<IActionResult> SeedRoles()
        // {
        //     bool isOwnerRoleExists = await roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
        //     bool isAdminRoleExists = await roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
        //     bool isManagerRoleExists = await roleManager.RoleExistsAsync(StaticUserRoles.MANAGER);
        //     bool isUserRoleExists = await roleManager.RoleExistsAsync(StaticUserRoles.USER);
        //     if (isOwnerRoleExists && isAdminRoleExists && isManagerRoleExists && isUserRoleExists)
        //     {
        //         return Ok(new { message = "Roles seeding is already done.", IsSucceed = true });
        //     }
        //     await roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
        //     await roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
        //     await roleManager.CreateAsync(new IdentityRole(StaticUserRoles.MANAGER));
        //     await roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
        //     return Ok(new { message = "Roles seeding done successfully.", IsSucceed = true });
        // }

        [HttpPost("register")]
        // public async Task<IActionResult> Register(RegisterDto dto) 可以,但是下面这样统一响应结构,swagger也能看到是返回什么类型
        public async Task<ActionResult<ApiResponseDto<object>>> Register(RegisterDto dto)
        {
            var isExistsUser = await userManager.FindByNameAsync(dto.UserName);
            if (isExistsUser != null)
            {
                // return BadRequest(new { message = "UserName already exists.", IsSucceed = false });
                return BadRequest(new ApiResponseDto<object> { Message = "UserName already exists.", IsSucceed = false });
            }

            //if not exists,then store dto info into sql
            var newUser = mapper.Map<ApplicationUser>(dto);
            var createUserResult = await userManager.CreateAsync(newUser, dto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User creation failed because:";
                foreach (var error in createUserResult.Errors)
                {
                    errorString = $"{errorString}# {error.Description}";
                }
                // return BadRequest(new { message = "UserName already exists.", IsSucceed = false });
                return BadRequest(new ApiResponseDto<object> { Message = "UserName already exists.", IsSucceed = false });
            }

            // add a default user role to all new users
            await userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);
            // create  a GetLogDto 
            var newLogDto = new GetLogDto { UserName = newUser.UserName, Description = "Register to website" };
            await logService.SaveNewLog(newLogDto);
            // return Ok(new { message = "UserName created successfully.", IsSucceed = true });
            return Ok(new ApiResponseDto<object> { Message = "UserName created successfully.", IsSucceed = true });
        }


        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login(LoginDto dto)
        {
            // 1 find user with username
            var user = await userManager.FindByNameAsync(dto.UserName);
            // 2 this username not exists
            if (user == null)
            {
                // return BadRequest(new { message = "UserName not exists,please register.", IsSucceed = false });
                return BadRequest(new ApiResponseDto<LoginResponseDto> { Message = "UserName not exists,please register.", IsSucceed = false });

            }
            // 3. username exists---this user exists, then check password of this user
            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordCorrect)
            {
                // return BadRequest(new { message = "Password is incorrect.", IsSucceed = false });
                return BadRequest(new ApiResponseDto<LoginResponseDto> { Message = "Password is incorrect.", IsSucceed = false });

            }

            //4 if username exists and password is correct->finally create token and userinfo to front-end
            // create  a GetLogDto 
            var newLogDto = new GetLogDto { UserName = user.UserName, Description = "New login" };
            await logService.SaveNewLog(newLogDto);

            var token = await tokenCreator.GenerateJWTToken(user);
            var userRoles = await userManager.GetRolesAsync(user);      // 因为我应用就是一个用户就一个角色,但是identity获取中间表的role的时候就只有roles的方法--所以我才用下面取出来第一个,就是转换成字符串(列表不能直接转换成单个字符串，需要先取元素，再处理可能为 null 的情况,所以才用first这个方法)
            string userRole = userRoles.FirstOrDefault() ?? StaticUserRoles.USER;
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

            return new ApiResponseDto<LoginResponseDto>
            {
                IsSucceed = true,
                Message = "Login successfully",
                Data = new LoginResponseDto
                {
                    Token = token,
                    User = userInfo
                }
            };
        }

        [HttpPost("update-role")]
        // 说ClaimsPrincipal User我是操作者, 如我是it管理员,我现在要更改某个员工的权限了--那我到底要改谁的, 改成什么样(这是UpdateRoleDto dto里的信息-操作者要做什么)
        // 要改谁？dto.UserName (这是你想要操作的目标用户的用户名)改成什么样？dto.NewRole (这是你想要设置的新角色)--所以这就是为什么UpdateRoleDto 有这两个属性
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> UpdateRole([FromBody] UpdateRoleDto dto)
        {
            // ClaimsPrincipal User
            // 当你启用认证（Authentication）时，通常在 Program.cs builder.Services.AddAuthentication... 这个
            // 那以后前端的请求中：
            // 用户携带 JWT token 或 Cookie
            // 框架会 拦截请求，解析 token / cookie
            // 验证成功后，会把用户信息封装成一个 ClaimsPrincipal 对象
            // 这个过程是 框架自动完成的，不需要你手动传---在 ControllerBase（以及 Controller 派生类）里，有一个属性public ClaimsPrincipal User { get; }, 这个控制器继承了ControllerBase
            // 那这个控制器也就有了这个属性了--所以在控制器任何地方都可以直接使用这个属性User了--这个属性的类型是一个类, 那我现在可以直接使用这个User--其实就是说我用的是 ClaimsPrincipal这个类的一个对象
            // 那我控制器到底是怎么拿到的不用管了, 只要知道是拿到了就行了

            // 1 find user with username.  拿到目标用户信息,我要改谁的角色
            var user = await userManager.FindByNameAsync(dto.UserName);
            // 2 this username not exists,验证目标用户存在
            if (user == null)
            {
                // return BadRequest(new { message = "Invalid UserName.", IsSucceed = false });
                return BadRequest(new ApiResponseDto<LoginResponseDto> { Message = "Invalid UserName.", IsSucceed = false });

            }
            // 3 just the owner and admin can update roles---这个是操作者要修改的用户的 现在现在数据库中存储的身份有哪些
            var userRoles = await userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault() ?? StaticUserRoles.USER;

            //4. 检查当前登录用户,是不是admin ---User.IsInRole("ADMIN"),结果是true/false
            // 执行角色修改
            //     如果管理员要把某个用户从 USER 改成 MANAGER，就允许。
            //     如果普通用户尝试改角色 → 拒绝
            // 下面这段 非常乱, 一定要分清楚, 操作者, 目标者 (操作者---更新目标者的角色身份信息 !!!!!!!!!!!!!)
            // step1操作者检查: User是操作者, 也就是当前登录的人, 看这个执行修改的人, 先要判断是不是有权限可以 更改身份 查操作者的权限!!!
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
                        return BadRequest(new ApiResponseDto<LoginResponseDto> { Message = "You are not alllowed to change role of this user.", IsSucceed = false });
                    }
                    else
                    {
                        // 那目标用户是比操作者低权限的角色--那 操作者可以修改这个人的身份
                        // 1. 移除所有旧角色： 将目标用户（user）当前拥有的所有角色全部删除--但是我应用就是一个用户一个角色,所以用RemoveFromRole 不用RemoveFromRoles
                        await userManager.RemoveFromRoleAsync(user, userRole);
                        await userManager.AddToRoleAsync(user, dto.NewRole);
                        // create  a GetLogDto 
                        var newLogDto = new GetLogDto { UserName = user.UserName, Description = "User role updated." };
                        await logService.SaveNewLog(newLogDto);
                        // return Ok(new { message = "Role updated successfully.", IsSucceed = true });
                        return Ok(new ApiResponseDto<LoginResponseDto> { Message = "Role updated successfully.", IsSucceed = true });
                    }
                }
                // return BadRequest(new { message = "You are not alllowed to change role of this user.", IsSucceed = false });
                return BadRequest(new ApiResponseDto<LoginResponseDto>{ Message = "You are not alllowed to change role of this user.", IsSucceed = false });
            }
            else
            {
                // 如果操作者是 owner(最高权限)
                // 那新角色检查: 只要不是修改的用户是owner这个角色, 那这个操作者都可以修改
                if (userRole == StaticUserRoles.OWNER)
                {
                    // return BadRequest(new { message = "You are not alllowed to change role of this user.", IsSucceed = false });
                    return BadRequest(new ApiResponseDto<LoginResponseDto> { Message = "You are not alllowed to change role of this user.", IsSucceed = false });
                }
                else
                {
                    // 目标者不是owner, 那我这个操作者就是随意的修改了
                    await userManager.RemoveFromRoleAsync(user, userRole);
                    await userManager.AddToRoleAsync(user, dto.NewRole);
                    // create  a GetLogDto 
                    var newLogDto = new GetLogDto { UserName = user.UserName, Description = "User role updated." };
                    await logService.SaveNewLog(newLogDto);
                    // return Ok(new { message = "Role updated successfully.", IsSucceed = true });
                    return Ok(new ApiResponseDto<LoginResponseDto>{ Message = "Role updated successfully.", IsSucceed = true });
                }
            }
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsersList()
        {
            var allUsers = await userManager.Users.ToListAsync();
            List<UserInfoDto> userDtos = new List<UserInfoDto>();

            foreach (var user in allUsers)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault();

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
            return Ok(new ApiResponseDto<List<UserInfoDto>> {Message="",IsSucceed=true,Data= userDtos});
        }
    }

}