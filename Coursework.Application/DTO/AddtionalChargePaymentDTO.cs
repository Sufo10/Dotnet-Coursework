using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class AdditonalChargePaymentDTO
    {
        [Required(ErrorMessage = "Additional Charge Id is required")]
        public string AddtionalChargeId { get; set; }
    }
}
