using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Core.Constants;
using WebApplication1.Core.Entities;

namespace WebApplication1.Core.Repositories
{
    public class TokenCreator
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public TokenCreator(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<string> GenerateJWTToken(ApplicationUser user)
        {
            
            var authClaims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name,user.UserName),
                 new Claim(ClaimTypes.NameIdentifier,user.Id),
                 new Claim("FirstName",user.FirstName),
                 new Claim("LastName",user.LastName),
                 new Claim(ClaimTypes.Role,userRole)
            };

            // foreach (var userRole in userRoles)
            // {
            //     authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            // }

            // !!!!!重要: 下面这3个都是实例化对象的,  new SymmetricSecurityKey(参数) :  构造函数赋值的方式 --进行实例化的--不是用的初始化器赋值的方式实例化的
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var singingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256); //用密钥+ 算法 进行加密--生成签名凭据-这就是jwt token的第三部分:签名

            var tokenObject = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: singingCredentials,
                claims: authClaims
            );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }
    }
}

// 1. 鼠标悬停在 Claim 上可以看到类型,或者ctrl +点击进去看源码
// 还可以: 看官方文档的"参数说明"
// // 当你写到这里：
// new JwtSecurityToken(
//     // 👈 按 Ctrl+Shift+Space 看参数提示
// IntelliSense 会告诉你：
// 哪些参数是必填的（没有 ? 标记）
// 哪些参数是可选的（有 ? 或 = null）


// 2. 怎么理解这个claims呢
// Claim 是一个"声明/断言"，它表示关于用户或实体的某个属性或信息。在身份验证和授权过程中，claims 用于携带用户的相关信息，以便系统能够根据这些信息做出访问控制决策。
// 姓名：张三
// 身份证号：123456

// 每一行都是一个 Claim（声明）：
// Type（类型）：这是什么信息？→ "姓名"、"身份证号"
// Value（值）：具体是什么？→ "张三"、"123456"
// ```bash
// // 声明1：我的名字是张三
// new Claim("姓名",  "张三")
// //        ↑Type    ↑Value
//       这是什么信息   具体值是什么
// // 声明2：我的身份证号是123456
// new Claim("身份证号", "123456")
// //        ↑Type     ↑Value
// ```


// Claim 就像一个"键值对"
// new Claim("键", "值")

// // 相当于字典
// Dictionary<string, string> = {
//     { "Name", "zhangsan" },
//     { "Id", "12345" }
// }

// 但 Claim 更专业，专门用于身份认证

// 3. 为什么需要它？统一 Key 的命名！, 微软定义了一些常用的key在 ClaimTypes 这个类里
// ❌ 不好：每个人自己写字符串，容易写错或不统一
// new Claim("name", user.UserName)           // 小写 name
// new Claim("Name", user.UserName)           // 大写 Name
// new Claim("userName", user.UserName)       // 驼峰 userName
// new Claim("user_name", user.UserName)      // 下划线 user_name

// ✅ 好：大家都用 ClaimTypes，保证统一
// new Claim(ClaimTypes.Name, user.UserName)  // 所有人都一样

// ClaimTypes 类的作用是提供一组预定义的常用声明类型（Claim Types）的常量。 这些常量代表了在身份验证和授权过程中常用的用户属性或信息类型，方便开发者在创建和处理声明时使用，避免手动输入字符串可能导致的错误或不一致性。
// 常用的 ClaimTypes 包括：
// ClaimTypes.Name              // 用户名
// ClaimTypes.NameIdentifier    // 用户ID
// ClaimTypes.Email             // 用户的电子邮件地址/邮箱
// ClaimTypes.Role              // 用户的角色
// ClaimTypes.GivenName         // 名字
// ClaimTypes.Surname           // 姓氏
// ClaimTypes.DateOfBirth       // 生日
// ClaimTypes.Country           // 国家
// ClaimTypes.MobilePhone      // 手机号码
// ClaimTypes.HomePhone        // 家庭电话号码
// ClaimTypes.StreetAddress    // 街道地址


// var authClaims = new List<Claim>
// {
//     // 标准属性：用 ClaimTypes
//     new Claim(ClaimTypes.Name, user.UserName),
//     new Claim(ClaimTypes.NameIdentifier, user.Id),
//     new Claim(ClaimTypes.Role, userRole),

//     // 自定义属性：用字符串
//     new Claim("CompanyId", "123"),
//     new Claim("Department", "IT部门")
// };

// 核心记忆点：
// Claim = 键值对（Type 是键，Value 是值）
// ClaimTypes = 预定义的标准键名（避免乱写字符串）
// 用 ClaimTypes 可以保证统一和专业


// 4. 办理 JWT Token 的完整流程--类比办身份证实体卡
// step1.收集个人信息 → authClaims(姓名、身份证号等)
// step2.准备防伪技术 → authSecret(芯片密钥)
// step3.设置签名方式 → SigningCredentials(用什么加密算法)
// step4.制作身份证   → JwtSecurityToken(把信息、密钥组合成卡)
// step5.发放给用户   → WriteToken(把卡片转成字符串，给用户)


// ==================== 第1步：收集用户信息 ====================
// 就像办身份证要填表格
// var authClaims = new List<Claim>
// {
//     new Claim(ClaimTypes.Name, user.UserName),        // 姓名
//     new Claim(ClaimTypes.NameIdentifier, user.Id),    // 身份证号
//     new Claim("FirstName", user.FirstName),           // 名字--还可以自定义
//     new Claim("LastName", user.LastName),             // 姓氏--还可以自定义
//     new Claim(ClaimTypes.Role, userRole)              // 角色（如：公民/警察）
// };

// // ==================== 第2步：准备"芯片密钥" ====================
// // 从配置文件读取密钥（appsettings.json 里的 "Jwt:Key"）
// // 就像身份证里的加密芯片
// var authSecret = new SymmetricSecurityKey(
//     Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
// );

// // ==================== 第3步：选择"加密算法--将密钥加密" ====================
// // 决定用什么方式签名（就像选择芯片加密技术）
// var signingCredentials = new SigningCredentials(
//     authSecret,                      // 用上面的密钥
//     SecurityAlgorithms.HmacSha256    // 用 HmacSha256 算法
// );

// // ==================== 第4步：制作"身份证" ====================
// // 把所有东西组装成一个 Token
// var tokenObject = new JwtSecurityToken(
//     issuer: configuration["Jwt:Issuer"],           // 发证机关（如：公安局）
//     audience: configuration["Jwt:Audience"],       // 适用范围（如：全国通用）
//     claims: authClaims,                            // 个人信息
//     expires: DateTime.Now.AddHours(2),             // 有效期（2小时后过期）
//     signingCredentials: signingCredentials         // 签名方式
// );

// // ==================== 第5步：转成字符串发给用户 ====================
// // 把"身份证"转成一串文字（Token字符串）
// var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

// // 返回给用户
// return token;  // 例如：eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIx...



// ## 📦 配置文件（appsettings.json）

// ```json
// {
//   "Jwt": {
//     "Key": "your-super-secret-key-at-least-32-characters-long",
//     "Issuer": "YourAppName",
//     "Audience": "YourAppUsers"
//   }
// }
// ```



// ## 🎯 记忆技巧
// ``` csharp
// 记住这个顺序：信息 → 密钥 → 签名 → 制作 → 发放
// Claim → Secret → Credentials → Token → WriteToken
// 或者记住：收集 → 加密 → 签名 → 组装 → 输出
// ```


// ## 🔑 必背的 5 个步骤（口诀）-- JWT 生成流程
// | 步骤 | 做什么 | 代码关键词 |
// |------|--------|-----------|
// | 1️⃣ | 收集信息 | `List<Claim>` |必须
// | 2️⃣ | 准备密钥 | `SymmetricSecurityKey` |必须
// | 3️⃣ | 设置签名 | `SigningCredentials` |必须
// | 4️⃣ | 制作Token | `JwtSecurityToken` |必须
// | 5️⃣ | 转成字符串 | `WriteToken` |必须

//    可选参数：
//    - issuer（发行者）---可以省略（但生产环境最好加上）
//    - audience（接收者）---可以省略（但生产环境最好加上）
//    - expires（过期时间）---不写就永不过期



// ## ✅ 完整模板（直接复制用）
// ```csharp
// public string GenerateJwtToken(User user, string userRole)
// {
//     // 1. 信息
//     var authClaims = new List<Claim>
//     {
//         new Claim(ClaimTypes.Name, user.UserName),
//         new Claim(ClaimTypes.NameIdentifier, user.Id),
//         new Claim(ClaimTypes.Role, userRole)
//     };

//     // 2. 密钥
//     var authSecret = new SymmetricSecurityKey(
//         Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
//     );

//     // 3. 签名
//     var signingCredentials = new SigningCredentials(
//         authSecret, 
//         SecurityAlgorithms.HmacSha256
//     );

//     // 4. 制作
//     var tokenObject = new JwtSecurityToken(
//         issuer: _configuration["Jwt:Issuer"],
//         audience: _configuration["Jwt:Audience"],
//         claims: authClaims,
//         expires: DateTime.Now.AddHours(2),
//         signingCredentials: signingCredentials
//     );

//     // 5. 输出(要把对象转成字符串)
//     return new JwtSecurityTokenHandler().WriteToken(tokenObject);
// }
// ```

// ---

// ## 💡 关键理解
// - **SymmetricSecurityKey**：对称密钥（就像芯片密码）
// - **SigningCredentials**：签名凭证（用什么方式加密）
// - **JwtSecurityToken**：Token对象（身份证实体）
// - **WriteToken**：序列化（把对象变成字符串）

// **记住 5 步：信息 → 密钥 → 签名 → 制作 → 输出** 🎯