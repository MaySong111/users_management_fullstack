namespace WebApplication1.Core.Dtos.Auth
{
    public class LoginResponseDto
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }
        public UserInfoDto User { get; set; }
    }
}