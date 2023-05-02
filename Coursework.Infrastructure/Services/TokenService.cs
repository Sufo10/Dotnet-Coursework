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
        // Fields to hold the JWT key, issuer, and audience, which are read from the app's configuration settings.
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        // Constructor to initialize the fields using IConfiguration.
        public TokenService(IConfiguration configuration)
        {
            _key = configuration.GetSection("JWT:Key").Value!;
            _issuer = configuration.GetSection("JWT:Issuer").Value!;
            _audience = configuration.GetSection("JWT:Audience").Value!;
        }

        // Method to generate a JWT token for an AppUser with a specific role.
        public string GenerateToken(AppUser user, string role)
        {
            // Define a list of claims that will be included in the token.
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role,role),
            new Claim(ClaimTypes.Email,user.Email)
        };

            // Define the security key and signing credentials using the JWT key.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set the expiration time for the token.
            var expires = DateTime.UtcNow.AddHours(12);

            // Create a new JWT token using the specified issuer, audience, claims, and signing credentials.
            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            // Return the JWT token as a string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
