using System.Security.Claims;
using WebApplication1.Core.Dtos;

namespace WebApplication1.Core.Services.Messages
{
    public interface IMessageService
    {
        Task<GeneralServiceResponseDto> CreateNewMessageAsync(CreateMessageDto dto, ClaimsPrincipal user);
        Task<IEnumerable<GetMessageDto>> GetMessagesAsync();
        Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal user);

    }
}