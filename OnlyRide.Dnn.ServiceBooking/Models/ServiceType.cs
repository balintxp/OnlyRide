using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace OnlyRide.Dnn.ServiceBooking.Models
{
    [TableName("OnlyRide_ServiceTypes")]
    [PrimaryKey("ServiceTypeId", AutoIncrement = true)]
    [Cacheable("OnlyRide_ServiceTypes", CacheItemPriority.Default, 20)]
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EstimatedMinutes { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
        public int ModuleId { get; set; }
    }
}