using System;
namespace Coursework.Presentation.Data.Models
{
	public class AdditionalCharges
	{
		public string Id { get; set; }
		public string CarId { get; set; }
		public string Description { get; set; }
		public double Amount { get; set; }
		public bool IsPaid { get; set; }
	}
}

