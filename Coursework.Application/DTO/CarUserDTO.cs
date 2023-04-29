using System;
using System;
namespace Coursework.Application.DTO
{
	public class CarUserDTO
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Double RatePerDay { get; set; }
        public Double ActualPrice { get; set; }
    }
}

