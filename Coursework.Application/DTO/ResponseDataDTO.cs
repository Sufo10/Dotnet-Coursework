using System;
using Coursework.Domain.Entities;

namespace Coursework.Application.DTO
{
	public class ResponseDataDTO:ResponseDTO
	{
		public List<Car> Data { get; set; } 
		
	}
}

