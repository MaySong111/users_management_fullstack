using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController(ILogService _logService) : ControllerBase
    {
        [HttpGet]
        // [Authorize(Roles = "OWNER,ADMIN")] 就和下面一行一样,但是用静态类就是为了avoid typing errors
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetLogs()
        {
            var logs = await _logService.GetLogsAsync();
            return Ok(logs);
        }

        [HttpGet("mine")]
        [Authorize]        // 只要是登录的用户, can access to this
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLogs()
        {
            // User 并不是你自己定义的变量，它是 ControllerBase 内置的属性。
            // ControllerBase.User   ControllerBase 有一个属性public ClaimsPrincipal User { get; },那继承了这个类的控制器也有这个属性

            var logs = await _logService.GetMyLogsAsync(User);
            if (logs == null)
            {
                return NotFound();
            }
            return Ok(logs);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLog(string UserName, string Description)
        {
            await _logService.SaveNewLogAsync(UserName, Description);
            return Ok();
        }
    }
}