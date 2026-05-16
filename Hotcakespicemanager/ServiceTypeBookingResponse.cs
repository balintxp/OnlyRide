using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakespicemanager
{
    public class ServiceTypeBookingResponse
    {
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public int EstimatedMinutes { get; set; }
        public int BookingCount { get; set; }
        public List<ServiceBooking> Bookings { get; set; }
    }

    public class ServiceBooking
    {
        public int BookingId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public decimal? ActualPrice { get; set; }
        public int? ActualMinutes { get; set; }
        public string CustomNote { get; set; }

        public string ServiceTypeName { get; set; }
    }
}
