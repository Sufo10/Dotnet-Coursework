using System;
namespace Coursework.Domain.Shared
{
	public class BaseEntity
	{
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? DeletedTime { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public Boolean? isDeleted { get; set; }
    }
}

