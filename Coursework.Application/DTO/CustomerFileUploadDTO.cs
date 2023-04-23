using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
