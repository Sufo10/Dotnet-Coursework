using System;
using System.ComponentModel.DataAnnotations;

namespace Coursework.Application.DTO
{
	public class CustomerRegisterRequestDTO
	{
		[Required(ErrorMessage ="Name is required")]

		public string? Name { get; set; }

		[EmailAddress]
		[Required(ErrorMessage ="Email is required")]

		public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]

        public string? Password { get; set; }

		[Required(ErrorMessage ="Confirm Password if required")]

		public string? ConfirmPassword { get; set; }

    }
}

