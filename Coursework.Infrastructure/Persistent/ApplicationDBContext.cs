using System;
using Coursework.Application.Common.Interface;
using Coursework.Domain.Entities;
using Coursework.Domain.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;



namespace Coursework.Infrastructure.Persistent
{
	public class ApplicationDBContext: IdentityDbContext<AppUser, IdentityRole, string>, IApplicationDBContext
	{
                 private readonly IDateTime _dateTime;
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options, IDateTime dateTime)
            : base(options)
        {
            _dateTime = dateTime;

        }
        public DbSet<CompanyEmployee> Employee { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerFileUpload> CustomerFileUpload { get; set; }
        public DbSet<Car> Car { get; set; }
        public DbSet<CustomerBooking> CustomerBooking { get; set; } 
        public DbSet<AdditionalCharges> AdditionCharges { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModified = _dateTime.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}

