namespace WebApplication1.Core.Dtos.Auth
{
    public class ApiResponseDto<T>
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}


// 本来我没定义--觉得没有必要--但是我在写前端的fetch, 要封装这个函数的时候--我判断各种情况--然后前端根据这个情况处理的时候--我发现真的很乱
// 这里的T可以是任何类型, 那new 这个dto的时候 Data =null, 也可以是 比如是LoginResponseDto 这个类, 那这样Data就是:
// Data: {
// Token:
// Id:
// ....
// }
// 总结我在ai中的解释
