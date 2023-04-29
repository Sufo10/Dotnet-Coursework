﻿using Coursework.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Domain.Entities
{
    public class AdditionalCharges: BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BookingId { get; set; }
        public string ChargeType { get; set; }
        public string Description { get; set; }
        public Double Amount { get; set; }
    }
}
