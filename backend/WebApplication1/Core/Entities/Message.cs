
namespace WebApplication1.Core.Entities
{
    public class Message : BaseEntity
    {
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public string Text { get; set; }
    }
}


