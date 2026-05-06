using System;

namespace OnlyRide.Dnn.ServiceBooking.Models
{
    public class Settings
    {
        // Hány héttel előre engedélyezett a foglalás (alapértelmezett: 4)
        public int MaxWeeks { get; set; } = 4;
    }
}