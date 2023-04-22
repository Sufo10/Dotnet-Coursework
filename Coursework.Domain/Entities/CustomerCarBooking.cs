using System;
using Coursework.Domain.Models;
using Coursework.Domain.Shared;

namespace Coursework.Domain.Entities
{
	public class CustomerCarBooking:BaseEntity
	{
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateOnly RentStartDate { get; set; }
        public DateOnly RentEndDate { get; set; }
        public RentStatus Status { get; set; } = RentStatus.Pending;
        public Boolean isDamaged { get; set; } = false;
        public Boolean isReturned { get; set; } = false;
        public DateOnly CarReturnedDate { get; set; }

        //public string 
    }
}

