using System;
namespace Coursework.Presentation.Data.Models
{
	public class AdminBooking
	{
		public string BookingId { get; set; }
		public string CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string CarName { get; set; }
		public string Image { get; set; }
		public string CarId { get; set; }
		public string RentStartDate { get; set; }
		public string RentEndDate { get; set; }
        public bool? payment { get; set; }
		public bool? IsCompleted { get; set; }
		public bool? IsAppoved { get; set; }
		public bool? IsDeleted { get; set; }
        public bool? OnRent { get; set; }
		public int TotalAmount { get; set; }

    }
}

