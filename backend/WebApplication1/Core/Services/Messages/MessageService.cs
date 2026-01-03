using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core.AppDbContext;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Entities;
using WebApplication1.Core.Interfaces;

namespace WebApplication1.Core.Services.Messages
{
    public class MessageService(ApplicationDbContext _context,
    UserManager<ApplicationUser> _userManager,
     ILogService _logService, IMapper _mapper) : IMessageService
    {
        public async Task<GeneralServiceResponseDto> CreateNewMessageAsync(CreateMessageDto dto, ClaimsPrincipal user)
        {
            var loggedInUser = user.Identity.Name;
            // 这个判断是一个安全验证, 为了防止用户给自己发消息 ---如果当前登录者用户名等于接收者的用户名,那说明用户想给自己发消息，这通常是被禁止的
            if (loggedInUser == dto.ReceiverUserName)
            {
                return new GeneralServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "Sender and receiver cannot be same,you cannot send message to yourself."
                };
            }
            var isReceiverUserNameValid = _userManager.Users.Any(u => u.UserName == dto.ReceiverUserName);
            if (!isReceiverUserNameValid)
            {
                return new GeneralServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "Receiver UserName is not valid."
                };
            }

            var newMessage = new Message
            {
                SenderUserName = loggedInUser,
                ReceiverUserName = dto.ReceiverUserName,
                Text = dto.Text
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await _logService.SaveNewLogAsync(loggedInUser, "Send message");
            return new GeneralServiceResponseDto
            {
                IsSucceed = true,
                Message = "Message saved successfully."
            };
        }


        public async Task<IEnumerable<GetMessageDto>> GetMessagesAsync()
        {
            var messagesDto = await _context.Messages.
            OrderByDescending(m => m.CreatedAt).ProjectTo<GetMessageDto>(_mapper.ConfigurationProvider).ToListAsync();

            return messagesDto;
        }


        public async Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal user)
        {
            var loggedInUser = user.Identity.Name;

            var messagesDto = await _context.Messages.
                    Where(m => m.SenderUserName == loggedInUser || m.ReceiverUserName == loggedInUser).
                    OrderByDescending(m => m.CreatedAt).
                    ProjectTo<GetMessageDto>(_mapper.ConfigurationProvider).
                    ToListAsync();

            return messagesDto;
        }
    }
}
