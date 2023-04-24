using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class BookCarRequestDTO
    {
        public string CarId { get; set; }
        public DateTime RentStartdate { get; set; }
        public DateTime RentEnddate { get; set; }
    }
}
