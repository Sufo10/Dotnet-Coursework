using System;
using Coursework.Application.Common.Interface;
using Coursework.Domain.Entities;

namespace Coursework.Infrastructure.Services
{
	public class CustomerService:ICustomerDetails
	{
		private readonly IApplicationDBContext _dbContext;

		public CustomerService(IApplicationDBContext dBContext)
		{
			_dbContext = dBContext;
		}
		public async Task<List<Customer>> GetAllCustomerService() {
			var data = _dbContext.Customer.Select(e => new Customer()
			{
				Id = e.Id,
				Name = e.Name,
				CustomerType = e.CustomerType
			}).ToList();
			return data;
		}
	}
}

