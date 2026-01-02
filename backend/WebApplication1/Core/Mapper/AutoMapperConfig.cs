using AutoMapper;
using WebApplication1.Core.Dtos;
using WebApplication1.Core.Entities;

namespace WebApplication1.Core.Mapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<GetLogDto, Log>();
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<Message, GetMessageDto>();
        }
    }
}