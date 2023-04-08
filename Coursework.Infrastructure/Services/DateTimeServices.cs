using System;
using Coursework.Application.Common.Interface;

namespace Coursework.Infrastructure.Services
{
	public class DateTimeServices:IDateTime
	{
        public DateTime Now => DateTime.UtcNow;
    }
}

