using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class BookingApproveRequestDTO
    {
        [Required (ErrorMessage = "customer id is required")]
        public string customerId { get; set; }  
    }
}
