using System;
namespace Coursework.Presentation.Data.Models
{
	public class SalesReportDetails
	{
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CarName { get; set; }
        public string CarId { get; set; }
        public string StartDate { get; set; }
        public string ApprovedBy { get; set; }
        public string EndDate { get; set; }
        public int Amount { get; set; }
    }
}

