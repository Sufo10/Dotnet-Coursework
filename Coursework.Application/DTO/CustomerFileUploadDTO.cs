using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Coursework.Application.DTO
{
	public class CustomerFileUploadDTO
	{
        [Required(ErrorMessage = "File is required")]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "FileType is required")]
        public string? FileType { get; set; }
    }
}

