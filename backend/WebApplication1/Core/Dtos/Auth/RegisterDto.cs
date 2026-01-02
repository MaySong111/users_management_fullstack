using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Core.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        // 对于简单的检查--更好的做法是直接在这里验证长度，比在 Program.cs(比如添加Configure<IdentityOptions>) 验证更早、更高效,  也比在控制器中判断好,节省了资源,这就是时机
        // 注意: 默认 Identity 配置要求至少一个大写字母、一个非字母符号 所以即便长度够 8，也会失败!!!!!!!!!!!!!
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }
        public string Address { get; set; }
    }
}