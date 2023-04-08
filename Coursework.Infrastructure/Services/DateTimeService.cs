using System;
using Coursework.Application.Common.Interface;

namespace Coursework.Infrastructure.Services
{
	public class DateTimeService:IDateTime
	{
        public DateTime Now => DateTime.UtcNow;
    }
}

