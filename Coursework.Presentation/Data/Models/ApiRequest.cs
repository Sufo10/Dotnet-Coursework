using System;
namespace Coursework.Presentation.Data.Models
{
    public class ApiResult<T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        // Add other properties if needed
    }

}

