using System;

namespace Coursework.Presentation.Data.Models

{
	public class Cars
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public Double RatePerDay { get; set; }
		public string Description { get; set; }
		public Double ActualPrice { get; set; }
    }
}

