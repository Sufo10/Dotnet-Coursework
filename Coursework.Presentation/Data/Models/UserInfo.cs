using System;
namespace Coursework.Presentation.Data.Models
{
    public class UserInfo
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsVerified { get; set; }
    }
}