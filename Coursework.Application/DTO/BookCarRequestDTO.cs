using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class BookCarRequestDTO
    {
        [Required (ErrorMessage = "car id is required")]
        public string CarId { get; set; }

        [Required(ErrorMessage = "Rent start date is required")]
        
        public DateTime RentStartdate { get; set; }

        [Required(ErrorMessage = "Rent end date is required")]
        public DateTime RentEnddate { get; set; }
    }
}






