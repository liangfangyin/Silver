using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Silver.OAuth
{
    public static class JwtUtil
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public static string IssuerSigningKey { get; set; } = "Irv65klnSBXXOlHKMOSh5IHzfZxfd79fPGSN4dB6KlrVNxP0x4vKxpOlSACEqO3S";

        /// <summary>
        /// 颁发令牌用户
        /// </summary>
        public static string ValidIssuer { get; set; } = "liangfy";

        /// <summary>
        /// 颁发机构
        /// </summary>
        public static string ValidAudience { get; set; } = "hangzhoupeak";

        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddJwt(this IServiceCollection services)
        {
            // 添加 JWT 认证服务
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; //生产环境必须为true
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //验证颁发者
                    ValidateAudience = true, //验证接收者
                    ValidateLifetime = true, //验证过期时间
                    ValidateIssuerSigningKey = true, //验证签名
                    ValidIssuer = JwtUtil.ValidIssuer,
                    ValidAudience = JwtUtil.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtUtil.IssuerSigningKey))
                };
            });
        }

        /// <summary>
        /// 使用服务
        /// </summary>
        /// <param name="app"></param>
        public static void UseJwt(this IApplicationBuilder app)
        {
            //启用身份验证
            app.UseAuthentication(); 
            app.UseAuthorization();
            //中间件验证token
            app.UseMiddleware<JwtMiddleware>();
        }

        /// <summary>
        /// JWT生成令牌
        /// </summary>
        /// <param name="subJect">key-val</param>
        /// <param name="hours">过期时长（小时）</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string CreateJwtToken(Dictionary<string, string> subJect, int hours = 2)
        {
            if (subJect == null || subJect.Count <= 0)
            {
                throw new Exception("subJect不能为空");
            }
            List<Claim> listClaim = new List<Claim>();
            foreach (string keys in subJect.Keys)
            {
                listClaim.Add(new Claim(keys, subJect[keys]));
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtUtil.IssuerSigningKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(listClaim),
                Expires = DateTime.UtcNow.AddHours(hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 获取token值
        /// </summary>
        /// <param name="token"></param>
        public static Dictionary<string, string> GetJwtToken(string token)
        {
            Dictionary<string, string> subJect = new Dictionary<string, string>();
            var tokenHandler = new JwtSecurityTokenHandler();
            var readToken = tokenHandler.ReadJwtToken(token);
            foreach(var claim in readToken.Claims) 
            {
                if (subJect.ContainsKey(claim.Type))
                {
                    continue;
                }
                subJect.Add(claim.Type, claim.Value);
            }
            return subJect;
        }


    }
}