using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class KhaltiPaymentDTO
    {
        [Required(ErrorMessage = "BookingId is required")]
        public string BookingId { get; set; }
    }
}
