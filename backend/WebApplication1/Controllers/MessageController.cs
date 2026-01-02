using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core.AppDbContext;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Entities;
using WebApplication1.Core.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationDbContext> userManager;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationDbContext> userManager, ILogService logService,IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.logService = logService;
            this.mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewMessage(CreateMessageDto dto)
        {
            // 这个判断是一个安全验证, 为了防止用户给自己发消息 ---如果当前登录者用户名等于接收者的用户名,那说明用户想给自己发消息，这通常是被禁止的
            if (User.Identity.Name == dto.ReceiverUserName)
            {
                return BadRequest(new { message = "Sender and receiver cannot be same,you cannot send message to yourself.", IsSucceed = false });
            }

            var isReceiverUserNameValid = await userManager.FindByNameAsync(dto.ReceiverUserName);
            if (isReceiverUserNameValid == null)
            {
                return BadRequest(new { message = "Receiver UserName is invalid.", IsSucceed = false });
            }

            var newMessage = new Message
            {
                SenderUserName = User.Identity.Name,
                ReceiverUserName = dto.ReceiverUserName,
                Text = dto.Text
            };

            context.Messages.Add(newMessage);
            await context.SaveChangesAsync();
            var newLog = new GetLogDto { UserName = User.Identity.Name, Description = "Send message" };
            await logService.SaveNewLog(newLog);
            return Ok(new { message = "Message saved successfully.", IsSucceed = true });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMessages()
        {
            var allMessages = await context.Messages.OrderByDescending(m => m.CreatedAt).ToListAsync();
            var convertedAllMessages  = mapper.Map<List<GetMessageDto>>(allMessages);
            return Ok(convertedAllMessages);
        }


        [HttpGet("mine")]
        public async Task<ActionResult<GetMessageDto>> GetMyMessages()
        {
            var loggedUser = User.Identity.Name;
            var allMyMessages = await context.Messages.Where(m => m.SenderUserName == loggedUser || m.ReceiverUserName == loggedUser).OrderByDescending(m => m.CreatedAt).ToListAsync();
            var convertedAllMyMessages  = mapper.Map<List<GetMessageDto>>(allMyMessages);
            return Ok(convertedAllMyMessages);
        }
    }

}
