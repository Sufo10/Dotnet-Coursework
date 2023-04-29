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
        public bool isPaid { get; set; }
		public int TotalAmount { get; set; }

    }
}

