using Coursework.Domain.Shared;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Domain.Entities
{
    public class CustomerBooking: BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string customer { get; set; }
        public string CarId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime RentStartdate { get; set; }
        public DateTime RentEnddate { get; set;}
        public Boolean IsApproved { get; set; } 
    }
}
