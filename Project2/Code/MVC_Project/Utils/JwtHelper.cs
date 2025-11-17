using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StorageCore.Utils
{
    /// <summary>
    /// JWT 工具类，用于生成和验证JWT令牌
    /// JWT Helper class for generating and validating JWT tokens
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数，注入配置服务
        /// Constructor with configuration injection
        /// </summary>
        /// <param name="configuration">配置服务 / Configuration service</param>
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 生成JWT令牌
        /// Generate JWT token
        /// </summary>
        /// <param name="userId">用户ID / User ID</param>
        /// <param name="role">用户角色 / User role</param>
        /// <param name="additionalClaims">额外的声明信息（可选） / Additional claims (optional)</param>
        /// <returns>生成的JWT令牌 / Generated JWT token</returns>
        public string GenerateToken(string userId, string role, IEnumerable<Claim> additionalClaims = null)
        {
            // 从配置中获取JWT密钥
            // Get JWT secret from configuration
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            // 创建基本声明（用户ID和角色）
            // Create basic claims (user ID and role)
            var claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("Role", role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // 唯一标识 / Unique identifier
            };

            // 添加额外声明（如果有）
            // Add additional claims if any
            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            // 创建令牌描述符
            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],               // 签发者 / Issuer
                Audience = _configuration["Jwt:Audience"],           // 受众 / Audience
                Subject = new ClaimsIdentity(claims),                // 声明集合 / Claims collection
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiresInMinutes"])), // 过期时间 / Expiration time
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature            // 签名算法 / Signing algorithm
                )
            };

            // 生成令牌
            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 返回令牌字符串
            // Return token string
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 验证JWT令牌并返回声明信息
        /// Validate JWT token and return claims
        /// </summary>
        /// <param name="token">JWT令牌 / JWT token</param>
        /// <returns>声明集合 / Claims collection</returns>
        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,    // 验证签名密钥 / Validate signing key
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,             // 验证签发者 / Validate issuer
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,           // 验证受众 / Validate audience
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,           // 验证有效期 / Validate expiration
                    ClockSkew = TimeSpan.Zero,         // 不允许时间偏移 / No time skew allowed
                    RequireExpirationTime = true       // 要求令牌包含过期时间 / Require expiration time
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                // 验证失败返回null
                // Return null if validation fails
                return null;
            }
        }

        /// <summary>
        /// 从令牌中获取声明值
        /// Get claim value from token
        /// </summary>
        /// <param name="token">JWT令牌 / JWT token</param>
        /// <param name="claimType">声明类型 / Claim type</param>
        /// <returns>声明值 / Claim value</returns>
        public string GetClaimFromToken(string token, string claimType)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var principal = ValidateToken(token);
            if (principal == null)
                return null;

            return principal.FindFirstValue(claimType);
        }

        /// <summary>
        /// 检查令牌是否过期
        /// Check if token is expired
        /// </summary>
        /// <param name="token">JWT令牌 / JWT token</param>
        /// <returns>是否过期 / Whether token is expired</returns>
        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true; // 解析失败视为过期 / Treat parsing failure as expired
            }
        }
    }
}