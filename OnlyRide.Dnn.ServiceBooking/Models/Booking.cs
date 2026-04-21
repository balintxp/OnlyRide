using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace OnlyRide.Dnn.ServiceBooking.Models
{
    [TableName("OnlyRide_Bookings")]
    [PrimaryKey("BookingId", AutoIncrement = true)]
    [Cacheable("OnlyRide_Bookings", CacheItemPriority.Default, 20)]
    [Scope("ModuleId")]
    public class Booking
    {
        public int BookingId { get; set; }
        public int SlotId { get; set; }
        public int UserId { get; set; }
        public int ServiceTypeId { get; set; }
        public string CustomNote { get; set; }
        public decimal EstimatedPrice { get; set; }
        public int? ActualMinutes { get; set; }
        public decimal? ActualPrice { get; set; }
        public string Status { get; set; } = "Függőben";
        public bool InvoiceSent { get; set; } = false;
        public DateTime CreatedOnDate { get; set; } = DateTime.UtcNow;
        public int ModuleId { get; set; }
    }
}