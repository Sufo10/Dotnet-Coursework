using System;
namespace Coursework.Presentation.Data.Models
{
	public class ResponseWithData<T>
	{
		public string status { get; set; }
		public string message { get; set; }
		public T data { get; set; }

    }
}

