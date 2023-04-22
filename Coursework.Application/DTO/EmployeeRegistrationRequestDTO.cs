using System;
using System.ComponentModel.DataAnnotations;

namespace Coursework.Application.DTO
{
	public class EmployeeRegistrationRequestDTO
	{
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Name is required")]

        public string? Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]

        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]

        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password if required")]

        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Address is required")]

        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]

        public string? Phone { get; set; }

        [Required(ErrorMessage ="Employee Type is required")]
        public string? EmployeeType { get; set; }
    }
}

