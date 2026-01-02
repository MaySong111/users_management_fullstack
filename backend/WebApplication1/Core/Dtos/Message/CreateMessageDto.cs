//发给谁,消息内容是什么, 那创建一个消息需要提供什么: 接收人,和消息内容, 根本不需要发送人用户名的
// 如果有发送人用户名的话: 会导致安全问题--恶意用户可以在前端篡改 SenderUserName，伪装成别人发送消息
// 因为一切身份相关的数据都由后端根据认证信息生成，前端只提供业务相关数据---User.Identity.Name 或者类似的身份验证对象已经记录了当前登录用户是谁
// 发送者信息是从登录状态获取的，不是由前端传过来的。

namespace WebApplication1.Core.Dtos
{
    public class CreateMessageDto
    {
        public string ReceiverUserName { get; set; }
        public string Text { get; set; }
    }
}