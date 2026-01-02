// this class will be used to avoid typing errors,而且方便统一修改
namespace WebApplication1.Core.Constants
{
    public static class StaticUserRoles
    {
        public const string OWNER = "OWNER";
        public const string ADMIN = "ADMIN";
        public const string MANAGER = "MANAGER";
        public const string USER = "USER";
        public const string OwnerAdmin = "OWNER,ADMIN";
        public const string OwnerAdminManager = "OWNER,ADMIN,MANAGER";
        public const string OwnerAdminManagerUser = "OWNER,ADMIN,MANAGER,USER";
    }

}

// 混淆点: --整个这些梳理清楚花了7个小时
//  这个定义就是为了打字错误, 方便统一管理, 比如一个控制器方法中authorize中允许某些角色访问, 那就是多个角色, 但是这个和 角色存储没关系的
// 这个应用中, 一个用户就一个角色,update也是一次选择一个角色,那这样的话 设计实体类的时候一个用户就是一个角色, 包括查询这个用户的信息UserInfoDto返回的角色也应该是一个啊
// 不要混淆这个
// 而且 前端要统计到底有多少角色显示到折线图,那是RoleCountDto之类的-- 但是除此之外的都 是一个用户一个角色啊, 整个视频混乱不堪
// 为什么视频各种用list/IEnumerable<string> : 就是因为操作中间表的时候
// 这是框架限制，没办法 : 
// var roles = await userManager.GetRolesAsync(user);  // IList<string>。  获取的就只有这个方法,类型就是list的, 所以接收的dto才定义的string的

// !!!!!!!!那还有一个办法, ---本来实体类不影响, 肯定存储的还是单个 那就定义单个string, 但是dto还是要定义单个, 至于从中间表返回的是数组--那取数组的第一个元素转换成字符串不就行了吗--然后赋值给这个dto的属性不就行了吗
// string userRole = roles.FirstOrDefault() ?? "USER";          用户可能 没有分配任何角色,FirstOrDefault() 在列表为空时返回 null,导致后续逻辑报错,所以才设置了一个默认的


// Identity 提供的 API
// await userManager.GetRolesAsync(user);        // 返回 IList<string>
// await userManager.AddToRoleAsync(user, role); // 添加一个角色, 也有roles的方法 添加多个
// await userManager.RemoveFromRoleAsync(user, roles); // 删除单个, 也有roles的方法删除多个的
// await userManager.IsInRoleAsync(user, role);  // 检查是否在某个角色






// 在一个系统中，不同用户能做的事情不同。
// 例如：
// 普通用户（USER）：只能看内容
// 管理员（ADMIN）：能删除、修改用户
// 经理（MANAGER）：能管理员工信息
// 系统所有者（OWNER）：拥有最高权限

// 系统角色等级如下：
// OWNER > ADMIN > MANAGER > USER
// 现在：
// “ADMIN” 登录进来，他是管理员；
// 他想改别人的角色。
// 那系统会判断：
// “你能不能把别人改成 OWNER？不行。”
// “你能不能改成 ADMIN？不行。”
// “改成 MANAGER 或 USER？行。”
// AuthController 文件中的if (updateRoleDto.NewRole == RoleType.USER || updateRoleDto.NewRole == RoleType.MANAGER)这句话就是表达上面的判断