using System;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Application.Common.Interface
{
	public interface IApplicationDBContext
	{
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

