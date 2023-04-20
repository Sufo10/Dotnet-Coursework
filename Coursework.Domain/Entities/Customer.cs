using System;
using Coursework.Domain.Models;
using Coursework.Domain.Shared;

namespace Coursework.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public CustomerType CustomerType { get; set; }
        public string Address { get; set; }
        public Boolean IsVerified { get; set; }
        public Guid UserId { get; set; }
        public Boolean PaymentFulfilled { get; set; }
        public string Phone { get; set; }
    }
}

