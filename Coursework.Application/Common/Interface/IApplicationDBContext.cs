using System;
using Coursework.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Application.Common.Interface
{
	public interface IApplicationDBContext
	{
        DbSet<Customer> Customer { get; set; }
        DbSet<CustomerFileUpload> CustomerFileUpload { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

