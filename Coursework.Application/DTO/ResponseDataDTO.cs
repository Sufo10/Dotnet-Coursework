using System;
using Coursework.Domain.Entities;

namespace Coursework.Application.DTO
{
	public class ResponseDataDTO<T>:ResponseDTO
	{
		public T Data { get; set; } 
		
	}
}

