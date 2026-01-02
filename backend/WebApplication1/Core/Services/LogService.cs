using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core.AppDbContext;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Entities;
using WebApplication1.Core.Interfaces;

namespace WebApplication1.Core.Repositories
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public LogService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task SaveNewLog(GetLogDto dto)
        {
            var newLog = mapper.Map<Log>(dto);
            context.Logs.Add(newLog);
            await context.SaveChangesAsync();
        }
        // 每当用户通过认证（比如 JWT token 登录成功）后，
        // 框架就会自动把这个用户的信息保存到一个对象里：
        // 👉 ClaimsPrincipal（也叫“用户声明主体”）
        // 它就像一个装着当前用户信息的容器，框架自动帮你注入---框架会自动注入当前登录用户的信息，不需要你手动传。 所以这里方法参数是这个, 框架会自动注入这个用户信息的

        // 所有人日志（可能是管理页面）
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetLogs()
        {
            var logs = await context.Logs.OrderByDescending(l => l.CreatedAt).ToListAsync();
            var convertedLogs = mapper.Map<List<GetLogDto>>(logs);
            return convertedLogs;
        }

        // 当前登录用户日志
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLog(ClaimsPrincipal User)
        {
            var logs = await context.Logs
               .Where(l => l.UserName == User.Identity.Name)
               .OrderByDescending(l => l.CreatedAt)
               .ToListAsync();

            var convertedLogs = mapper.Map<List<GetLogDto>>(logs);
            return convertedLogs;
        }
    }
}

// 这里整段: 复制给claude, 接口, 以及控制器的方法--然后让claude 总结   -=很重要
// 当用户登录后，你在 JWT 里包含了这些信息：
// {
//   "sub": "1234",
//   "unique_name": "john",
//   "role": "Admin"
// }
// ASP.NET Core 解析这个 token 后，就会自动生成一个 ClaimsPrincipal 对象，里面长这样
// user.Identity.Name == "john"
// user.FindFirst(ClaimTypes.NameIdentifier)?.Value == "1234"
// user.FindFirst(ClaimTypes.Role)?.Value == "Admin"

// 所以在控制器中调用GetMyLog函数的时候--控制器中方法: GetMyLogs 中logRepository.GetMyLog(User); 这里的参数User是:
// 用户登录，拿到 JWT 或 Cookie。
// 用户请求 GET /mine，请求头里带着 Token。
// ASP.NET Core 验证 Token，生成 ClaimsPrincipal。
// 框架把这个对象赋值给 ControllerBase 这个属性----然后控制器继承了ControllerBase --所以 Controller 也有User 属性。



// ClaimsPrincipal 的结构简单理解为下面的:
// ClaimsPrincipal user = new ClaimsPrincipal
// {
//     Identity = new ClaimsIdentity(new[]
//     {
//         new Claim(ClaimTypes.Name, "john"),
//         new Claim(ClaimTypes.Role, "Admin"),
//         new Claim(ClaimTypes.Email, "john@example.com")
//     })
// };
// 所以可以直接在方法中使用下面的方法:
// | 方法                                                 | 代表什么意思                                                |
// | -------------------------------------------------- | ----------------------------------------------------- |
// | `user.Identity.Name`                               | 用户名，比如 "john"                                         |
// | `user.FindFirst(ClaimTypes.NameIdentifier)?.Value` | 用户 ID，比如 "1234"                                       |
// | `user.FindFirst(ClaimTypes.Role)?.Value`           | 用户角色，比如 "Admin"                                       |
// | `user.IsInRole("Admin")`                           | 判断是不是管理员，返回 true / false                              |
// | `user.FindFirst(ClaimTypes.Email)?.Value`          | 用户邮箱，比如 "[john@example.com](mailto:john@example.com)" |

// [Authorize]
// [HttpGet("profile")]
// public IActionResult GetProfile(ClaimsPrincipal user)
// {
//     var name = user.Identity.Name;                     // john
//     var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // 1234
//     var isAdmin = user.IsInRole("Admin");               // true

//     return Ok(new { name, id, isAdmin });
// }
