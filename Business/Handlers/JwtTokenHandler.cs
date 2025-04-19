using Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Handlers
{
    public interface IJwtTokenHandler
    {
        string GenerateJwtToken(UserEntity user, string? role = null);
    }

    public class JwtTokenHandler(IConfiguration configuration) : IJwtTokenHandler
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateJwtToken(UserEntity user, string? role = null)
        {

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);
            var issuer = _configuration["Jwt:Issuer"]!;
            var claims = GetClaims(user, role);

            var tokenDescriptor = CreateTokenDescriptor(key, issuer, claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public List<Claim> GetClaims(UserEntity user, string? role = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
            };

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                if (role == "Admin")
                    claims.Add(new Claim("apiKey", _configuration["SecretKeys:Admin"]!));
            }

            return claims;
        }

        public SecurityTokenDescriptor CreateTokenDescriptor(byte[] key, string issuer, List<Claim> claims)
        {
            return new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}

