using System;
using Coursework.Domain.Shared;

namespace Coursework.Domain.Entities
{
	public class Car:BaseEntity
	{
		public Guid Id { get; set; } = Guid.NewGuid(); 
		public string Name { get; set; }
		public Boolean IsAvailable { get; set; }
		public Double RatePerDay { get; set; }
		public string Image { get; set; }
		public string Description { get; set; }
	}
}

