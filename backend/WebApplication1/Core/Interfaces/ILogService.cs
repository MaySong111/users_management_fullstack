using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Dtos;

namespace WebApplication1.Core.Interfaces
{
    public interface ILogService
    {
        Task SaveNewLog(GetLogDto dto);
        Task<ActionResult<IEnumerable<GetLogDto>>> GetLogs();
        Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLog(ClaimsPrincipal User);
    }
}