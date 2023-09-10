using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Peak.Lib.Basics;


namespace Silver.OAuth
{
    public class JwtMiddleware
    {
        private const string AuthenticationHeader = "Authorization";
        private const string AuthenticationScheme = "Bearer";
        private const string LoginoutHeader = "Loginout";//退出登录的Header
        private const int CacheExpiration = 36000;//缓存过期时间 秒，不解析jwt数据，直接将缓存时间设置为Token的最大有效时间，以空间换性能
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;

        public JwtMiddleware(RequestDelegate _next, IDistributedCache _cache)
        {
            this.next = _next;
            this.cache = _cache;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var auth = context.Request.Headers[AuthenticationHeader].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith(AuthenticationScheme))
            {
                var token = auth.Substring(AuthenticationScheme.Length).Trim();
                var cacheKey = $"TokenBlack:{token.ToMD5_32()}";
                if (JwtUtil.GetJwtToken(token).Count <= 0)
                {
                    context.Response.WriteAsync((new { code = 8, msg = "token无效" }).ToJson());
                    return;
                }
                //将Token的摘要写入黑名单
                if (context.Request.Headers[LoginoutHeader].FirstOrDefault() != null)//只要带了LoginoutHeader并且value不为null，就认为是要记录黑名单
                {
                    await cache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheExpiration)
                    });
                    context.Response.StatusCode = 200;
                    return;
                }
                //如果是黑名单，直接返回401
                else if ((await cache.GetStringAsync(cacheKey)) != null)
                { 
                    context.Response.WriteAsync((new { code = 8, msg = "token已过期退出" }).ToJson());
                    return;
                }
            }
            await next(context);
        }

    }
}
