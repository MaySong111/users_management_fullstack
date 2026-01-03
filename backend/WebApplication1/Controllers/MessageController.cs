using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Services.Messages;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController(IMessageService _messageService) : ControllerBase
    {

        [HttpPost("create")]
        public async Task<ActionResult> CreateNewMessage(CreateMessageDto dto)
        {

            var result = await _messageService.CreateNewMessageAsync(dto, User);
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMessages()
        {
            var messages = await _messageService.GetMessagesAsync();
            return Ok(messages);
        }


        [HttpGet("mine")]
        public async Task<ActionResult<GetMessageDto>> GetMyMessages(ClaimsPrincipal user)
        {
            var messages = await _messageService.GetMyMessagesAsync(user);
            return Ok(messages);
        }
    }

}
