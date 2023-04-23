using System;
using Coursework.Domain.Shared;

namespace Coursework.Domain.Entities
{
	public class CustomerFileUpload:BaseEntity
	{
        public Guid Id { get; set; } = new Guid();
        public string FileName { get; set; }
        public string DocumentType { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}

