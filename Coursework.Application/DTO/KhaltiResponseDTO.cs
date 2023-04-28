using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    public class KhaltiResponseDTO
    {
        public string? pidx { get; set; }
        public string? payment_url { get; set; } 
        public string? error { get; set; }
        public string? total_amount { get; set; }
        public string? status { get; set; }
    }
}
