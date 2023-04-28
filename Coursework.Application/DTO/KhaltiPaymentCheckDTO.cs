using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class KhaltiPaymentCheckDTO
    {
        [Required(ErrorMessage = "Pidx is required")]
        public string pidx { get; set; }
    }
}
