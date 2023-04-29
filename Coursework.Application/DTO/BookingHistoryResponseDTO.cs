using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.DTO
{
    //public class CarDTO
    //{
    //    public Guid Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string Image { get; set; }
    //    public double RatePerDay { get; set; }
    //}
    //public class CarBookDTO
    //{
    //    public Guid Id { get; set; }
    //    public string customerId { get; set; }
    //    public string CarId { get; set; }
    //    public DateTime RentStartdate { get; set; }
    //    public DateTime RentEnddate { get; set; }
    //}
    public class BookingHistoryResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        //public string customerId { get; set; }
        public string Description { get; set; }
        public DateTime RentStartdate { get; set; }
        public DateTime RentEnddate { get; set; }
        public string ApprovedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsPaid { get; set; }
        public Double? TotalAmount { get; set; }
        public bool? OnRent { get; set; }
        public bool? IsCompleted { get; set; }
    }

    public class SalesRecordResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApprovedBy { get; set; }
    }
}
