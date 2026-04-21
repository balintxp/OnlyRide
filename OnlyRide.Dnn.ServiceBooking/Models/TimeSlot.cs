using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace OnlyRide.Dnn.ServiceBooking.Models
{
    [TableName("OnlyRide_TimeSlots")]
    [PrimaryKey("SlotId", AutoIncrement = true)]
    [Cacheable("OnlyRide_TimeSlots", CacheItemPriority.Default, 20)]
    [Scope("ModuleId")]
    public class TimeSlot
    {
        public int SlotId { get; set; }
        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int ModuleId { get; set; }
    }
}