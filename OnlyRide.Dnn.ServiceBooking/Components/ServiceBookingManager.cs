/*
' Copyright (c) 2026 OnlyRide
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Data;
using DotNetNuke.Framework;
using OnlyRide.Dnn.ServiceBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlyRide.Dnn.ServiceBooking.Components
{
    internal interface IServiceBookingManager
    {
        // --- Foglalások (Bookings) ---
        void CreateBooking(Booking b);
        void UpdateBooking(Booking b);
        void DeleteBooking(Booking b);
        Booking GetBooking(int bookingId, int moduleId);
        IEnumerable<Booking> GetBookings(int moduleId);
        IEnumerable<Booking> GetBookingsByUser(int userId, int moduleId);

        // --- Szerviz típusok (ServiceTypes) ---
        IEnumerable<ServiceType> GetServiceTypes(int moduleId);
        ServiceType GetServiceType(int serviceTypeId, int moduleId);

        // --- Időpontok (TimeSlots) ---
        IEnumerable<TimeSlot> GetTimeSlots(int moduleId);
        TimeSlot GetTimeSlot(int slotId, int moduleId);
        void UpdateTimeSlot(TimeSlot t);

        // --- Járművek (Vehicles) ---
        void CreateVehicle(Vehicle v);
        void UpdateVehicle(Vehicle v);
        Vehicle GetVehicleByBooking(int bookingId);
    }

    internal class ServiceBookingManager : ServiceLocator<IServiceBookingManager, ServiceBookingManager>, IServiceBookingManager
    {
        // ==========================================
        // BOOKINGS (Foglalások)
        // ==========================================
        public void CreateBooking(Booking b)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                rep.Insert(b);
            }
        }

        public void UpdateBooking(Booking b)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                rep.Update(b);
            }
        }

        public void DeleteBooking(Booking b)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                rep.Delete(b);
            }
        }

        public Booking GetBooking(int bookingId, int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                return rep.GetById(bookingId, moduleId);
            }
        }

        public IEnumerable<Booking> GetBookings(int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                return rep.Get(moduleId);
            }
        }

        public IEnumerable<Booking> GetBookingsByUser(int userId, int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                // PDF előírás szerinti paraméterezett SQL keresés a SQL injection ellen
                return rep.Find("WHERE UserId = @0 AND ModuleId = @1", userId, moduleId);
            }
        }

        // ==========================================
        // SERVICE TYPES (Szerviz típusok)
        // ==========================================
        public IEnumerable<ServiceType> GetServiceTypes(int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceType>();
                return rep.Get(moduleId);
            }
        }

        public ServiceType GetServiceType(int serviceTypeId, int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceType>();
                return rep.GetById(serviceTypeId, moduleId);
            }
        }

        // ==========================================
        // TIME SLOTS (Időpontok)
        // ==========================================
        public IEnumerable<TimeSlot> GetTimeSlots(int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSlot>();
                return rep.Get(moduleId);
            }
        }

        public TimeSlot GetTimeSlot(int slotId, int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSlot>();
                return rep.GetById(slotId, moduleId);
            }
        }

        public void UpdateTimeSlot(TimeSlot t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSlot>();
                rep.Update(t);
            }
        }

        // ==========================================
        // VEHICLES (Járművek)
        // ==========================================
        public void CreateVehicle(Vehicle v)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                rep.Insert(v);
            }
        }

        public void UpdateVehicle(Vehicle v)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                rep.Update(v);
            }
        }

        public Vehicle GetVehicleByBooking(int bookingId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                return rep.Find("WHERE BookingId = @0", bookingId).FirstOrDefault();
            }
        }

        protected override System.Func<IServiceBookingManager> GetFactory()
        {
            return () => new ServiceBookingManager();
        }
    }
}