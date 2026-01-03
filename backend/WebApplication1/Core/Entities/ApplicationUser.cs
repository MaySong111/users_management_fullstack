using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // public string Role { get; set; } 不是这个string,也不是string[]数组固定长度的, 而是 list<T> 长度不固定, 这样设计 是说我的这个应用  
        // 一个用户就一个角色, 所以是string不是list也不是数组---但是我这个用户的也没有必要定义一个属性role啊, 都是根据UserName属性来找中间表的这种方式获取的用户的角色的啊
        // public string Role { get; set; }
    }
}