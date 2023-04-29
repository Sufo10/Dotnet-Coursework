using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class GetMostRentalRequestDTO
    {
        
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public int NoOfRequest { get; set; }

    }
}
