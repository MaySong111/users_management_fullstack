namespace WebApplication1.Core.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserInfoDto User { get; set; }
    }
}