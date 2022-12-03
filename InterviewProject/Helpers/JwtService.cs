using InterviewProject.Shared;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterviewProject.Helpers
{
    public class JwtService
    {
        // TODO: Remove
        private readonly string secretKey;

        public JwtService(string secretKey)
        {
            this.secretKey = secretKey;
        }

        public string Generate(int id, AuthRoleType authRoleType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Role, authRoleType.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken) validatedToken;
        }
    }
}
