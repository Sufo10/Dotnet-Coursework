using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Infrastructure.Services
{
    public static class UtilityService
    {
        public static void ValidateIdentityResult(IdentityResult result)
        {
             if (result.Succeeded) return;
            var errors = result.Errors.Select(x => x.Description);
             throw new Exception(string.Join('\n', errors));
        }
        public static string ToUrlSafeBase64(string base64String)
        {
            return base64String.Replace('+', '-').Replace('/', '~').Replace('=', '_');
        }
        public static string FromUrlSafeBase64(string urlSafeBase64String)
        {
            return urlSafeBase64String.Replace('-', '+').Replace('~', '/').Replace('_', '=');
        }
    }
}
