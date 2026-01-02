using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.AppDbContext;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Entities;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;

        public TestController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.context = context;
        }
        [HttpGet("get-public")]
        public async Task<IActionResult> GetPublicData()
        {
            return Ok(new { message = "Public data" });
        }

        [HttpGet("get-user-role")]
        [Authorize(Roles = StaticUserRoles.USER)]
        public async Task<IActionResult> GetUserData()
        {
            return Ok(new { message = "User role data" });
        }

        [HttpGet("get-manager-role")]
        [Authorize(Roles = StaticUserRoles.MANAGER)]
        public async Task<IActionResult> GetManagerData()
        {
            return Ok(new { message = "Manager role data" });
        }

        [HttpGet("get-admin-role")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public async Task<IActionResult> GetAdminData()
        {
            return Ok(new { message = "Admin role data" });
        }

        [HttpGet("get-owner-role")]
        [Authorize(Roles = StaticUserRoles.OWNER)]
        public async Task<IActionResult> GetOwnerData()
        {
            return Ok(new { message = "Owner role data" });
        }
    }
}