using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities.Identity;
using TaskManager.Core.Services;

namespace TaskManager.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser User ,UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            //Payload
            //1. Private Claims [User - Defined]
            var AuthClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.GivenName, User.DisplayName),
                new Claim(JwtRegisteredClaimNames.Email, User.Email)
            };
            var userClaims = await userManager.GetClaimsAsync(User);
            foreach (var claim in userClaims)
            {
                AuthClaims.Add(claim);
            }
            var UserRoles = await userManager.GetRolesAsync(User);
            foreach (var UserRole in UserRoles)
            {
                var Role = await roleManager.FindByNameAsync(UserRole);
                if(Role != null)
                {
                    AuthClaims.Add(new Claim("role", UserRole));
                    var RoleClaims = await roleManager.GetClaimsAsync(Role);    // add claims to the role
                    foreach(var RoleClaim in RoleClaims)
                    {
                        AuthClaims.Add(RoleClaim);
                    }
                }

            }

            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var Token = new JwtSecurityToken(
                 issuer: configuration["JWT:ValidIssuer"],
                 audience: configuration["JWT:ValidAudience"],
                 expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationsInDays"])),
                 claims: AuthClaims,
                 signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
                 );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
