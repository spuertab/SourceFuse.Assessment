using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SourceFuse.Assessment.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SourceFuse.Assessment.Common.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Login(LoginModel model)
        {
            if ((model.Username == "spuertab1" || model.Username == "spuertab2") && model.Password == "123")
            {
                string[] userRoles;
                if (model.Username == "spuertab1")
                {
                    userRoles = ["Admin", "User"];
                }
                else
                {
                    userRoles = ["User"];
                }

                return GenerateJwtToken(model.Username, userRoles);
            }

            return null;
        }

        private string GenerateJwtToken(string username, string[] roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
