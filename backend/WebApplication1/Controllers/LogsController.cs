using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService logService;
        public LogsController(ILogService logService)
        {
            this.logService = logService;
        }

        [HttpGet]
        // [Authorize(Roles = "OWNER,ADMIN")] 就和下面一行一样,但是用静态类就是为了avoid typing errors
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetLogs()
        {
            var logs = await logService.GetLogs();
            return Ok(logs);
        }

        [HttpGet("mine")]
        [Authorize]        // 只要是登录的用户, can access to this
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLogs()
        {
            // User 并不是你自己定义的变量，它是 ControllerBase 内置的属性。
            // ControllerBase.User   ControllerBase 有一个属性public ClaimsPrincipal User { get; },那继承了这个类的控制器也有这个属性了
            var logs = logService.GetMyLog(User);
            if (logs == null)
            {
                return NotFound();
            }
            return Ok(logs);
        }
    }
}