using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Domain.Models;
using Coursework.Domain.Shared;

namespace Coursework.Domain.Entities
{
	public class CompanyEmployee:BaseEntity
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }
        public string Address { get; set; }
        public Boolean IsVerified { get; set; }
		public Boolean PaymentFulfilled { get; set; }
		public string Phone { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}

