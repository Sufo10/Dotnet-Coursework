using System;
namespace Coursework.Application.DTO
{
	public class ErrorMessageResponse
	{
        public string? Message { get; set; }
        public string? ContentType { get; set; }
        public int StatusCode { get; set; }
    }
}

