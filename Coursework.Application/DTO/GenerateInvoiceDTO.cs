using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class GenerateInvoiceDTO
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CarName { get; set; }
        public double RatePerDay { get; set; }
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public double RentalAmount { get; set; }
        public double VATAmount { get; set; }
        public string Message { get; set; }
    }
}
