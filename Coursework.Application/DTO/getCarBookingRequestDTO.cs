using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class GetCarBookingRequestDTO
    {
        public string BookingId { get; set; }   
        public string CustomerId {  get; set; } 
        public string CustomerName {  get; set; } 
        public string CustomerPhone { get; set; }
        public string CarId { get; set; }
        public DateTime RentStartdate { get; set; }
        public DateTime RentEnddate { get;  set; }

    }
}
