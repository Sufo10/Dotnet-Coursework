using System;
namespace Coursework.Presentation.Data.Models
{
	public class PaymentResponse
	{
        public string? pidx { get; set; }
        public string? payment_url { get; set; }
        public string? error { get; set; }
        public string? total_amount { get; set; }
        public string? status { get; set; }
    }
}

