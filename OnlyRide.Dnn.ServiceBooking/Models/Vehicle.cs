using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace OnlyRide.Dnn.ServiceBooking.Models
{
    [TableName("OnlyRide_Vehicles")]
    [PrimaryKey("VehicleId", AutoIncrement = true)]
    [Cacheable("OnlyRide_Vehicles", CacheItemPriority.Default, 20)]
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public int BookingId { get; set; }
        public string VehicleType { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
    }
}