using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Coursework.Application.Common.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Identity;
using Coursework.Domain.Entities;

namespace Coursework.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        public TokenService(IConfiguration configuration)
        {
            _key = configuration.GetSection("JWT:Key").Value!;
            _issuer = configuration.GetSection("JWT:Issuer").Value!;
            _audience = configuration.GetSection("JWT:Audience").Value!;
        }

        //public string GenerateToken(IdentityUser user, string role)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_key);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[] {
        //            new Claim(ClaimTypes.NameIdentifier, user.Id),
        //            new Claim(ClaimTypes.Name, user.UserName),
        //             new Claim(ClaimTypes.Role, role)
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(12),
        //        Issuer = _issuer,
        //        Audience = _audience,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
        //    SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(securityToken);
        //}

        public string GenerateToken(AppUser user,string role)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Role,role)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(12);
            var token = new JwtSecurityToken(
                _issuer,
               _audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
