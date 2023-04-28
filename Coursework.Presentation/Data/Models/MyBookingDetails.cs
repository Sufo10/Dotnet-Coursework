using System;
namespace Coursework.Presentation.Data.Models
{
	public class MyBookingDetails
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public string RentStartDate { get; set; }
		public string RentEndDate { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsApproved { get; set; }
	}
}

