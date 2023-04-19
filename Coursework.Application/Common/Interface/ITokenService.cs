using Coursework.Application.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.Common.Interface
{
    public interface ITokenService
    {
        public string GenerateToken(IdentityUser model, string role);
    }
}
