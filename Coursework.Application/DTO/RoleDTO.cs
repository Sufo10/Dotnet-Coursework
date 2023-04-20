using System;
using System.ComponentModel.DataAnnotations;

namespace Coursework.Application.DTO
{
	public class RoleDTO
	{
        [Required(ErrorMessage = "Name is required")]

        public string? Name { get; set; }
    }
}

