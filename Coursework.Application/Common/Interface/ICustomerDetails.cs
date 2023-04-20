using System;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICustomerDetails
	{
		Task<List<Customer>> GetAllCustomerService();
	}
}

