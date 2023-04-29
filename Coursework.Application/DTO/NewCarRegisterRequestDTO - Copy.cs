using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Coursework.Application.DTO
{
	public class EditCarRequestDTO
	{
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        public Double? RatePerDay { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage ="Image is required")]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        public Double ActualPrice { get; set; }

    }
}

