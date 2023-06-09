﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class UserChangePasswordDTO
    {
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password if required")]
        public string? ConfirmPassword { get; set; }
    }
}
