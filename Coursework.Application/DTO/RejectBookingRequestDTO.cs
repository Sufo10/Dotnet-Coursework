using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class RejectBookingRequestDTO
    {
        [Required(ErrorMessage = "booking id is required")]
        public string BookingId { get; set; }
    }
}
