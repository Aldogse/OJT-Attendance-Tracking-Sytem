using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Attendace_Tracking_Sytem.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<LogInCredentials> _userManager;

        public JwtService(IConfiguration config,UserManager<LogInCredentials>userManager)
        {
            _config = config;
            _userManager = userManager;
        }
        public async Task<string> GenerateToken(LogInCredentials user)
        {
            //GET USER ROLES
            var roles = await _userManager.GetRolesAsync(user);

            //GET USER CLAIMS
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),            
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            //HASH THE KEY 
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Token"]));

            //GET THE CREDENTIALS 
            SigningCredentials credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            //GENERATE THE TOKEN
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuers"],
                audience: _config["Jwt:Audiences"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
