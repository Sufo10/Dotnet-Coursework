using System;
using Coursework.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Application.Common.Interface
{
	public interface IApplicationDBContext
	{
        DbSet<CompanyEmployee> Employee { get; set; }
        DbSet<Customer> Customer { get; set; }
        DbSet<CustomerFileUpload> CustomerFileUpload { get; set; }
        DbSet<Car> Car { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

