using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Common;
using OnlyRide.Dnn.ServiceBooking.Components;
using OnlyRide.Dnn.ServiceBooking.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace OnlyRide.Dnn.ServiceBooking.Controllers
{
    [DnnHandleError]
    public class BookingController : DnnController
    {
        public ActionResult Delete(int bookingId)
        {
            var booking = ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);
            if (booking != null)
            {
                ServiceBookingManager.Instance.DeleteBooking(booking);
            }
            return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
        }

        public ActionResult Edit(int bookingId = -1, string date = "")
        {

            ViewBag.ServiceTypes = ServiceBookingManager.Instance.GetServiceTypes(ModuleContext.ModuleId);

            var booking = (bookingId == -1 || bookingId == 0)
                ? new Booking
                {
                    ModuleId = ModuleContext.ModuleId,
                    CreatedOnDate = !string.IsNullOrEmpty(date) ? DateTime.Parse(date) : DateTime.Now,
                    Status = "Függőben"
                }
                : ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);
            // Vehicle adatok betöltése ha már létező foglalás
            if (booking.BookingId > 0)
            {
                ViewBag.Vehicle = ServiceBookingManager.Instance.GetVehicleByBooking(booking.BookingId);
            }

            return View(booking);
        }

        [HttpPost]
        //[DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }
            try
            {
                if (booking.BookingId <= 0)
                {
                    booking.UserId = User.UserID;
                    booking.ModuleId = ModuleContext.ModuleId;
                    booking.SlotId = booking.SlotId > 0 ? booking.SlotId : 0;
                    booking.ServiceTypeId = booking.ServiceTypeId > 0 ? booking.ServiceTypeId : 1;
                    booking.EstimatedPrice = 0;
                    booking.Status = "Függőben";
                    ServiceBookingManager.Instance.CreateBooking(booking);

                    if (booking.BookingId > 0)
                    {
                        var vehicle = new Vehicle
                        {
                            BookingId = booking.BookingId,
                            VehicleType = Request.Form["VehicleType"],
                            Brand = Request.Form["Brand"],
                            Model = Request.Form["VehicleModel"],
                            SerialNumber = Request.Form["SerialNumber"],
                            Notes = Request.Form["VehicleNotes"]
                        };
                        ServiceBookingManager.Instance.CreateVehicle(vehicle);
                    }
                }
                else
                {
                    var existing = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);
                    if (existing != null)
                    {
                        existing.ServiceTypeId = booking.ServiceTypeId;
                        existing.CustomNote = booking.CustomNote;
                        ServiceBookingManager.Instance.UpdateBooking(existing);
                    }
                }
                return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
            }
            catch (Exception ex)
            {
                return Content("MENTÉSI HIBA TÖRTÉNT: " + ex.Message);
            }
        }
        public ActionResult Index(int weekOffset = 0)
        {
            var bookings = ServiceBookingManager.Instance.GetBookings(ModuleContext.ModuleId);
            ViewBag.WeekOffset = weekOffset;
            return View(bookings);
        }

        public ActionResult Cancel(int bookingId)
        {
            var booking = ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);
            if (booking != null && (booking.UserId == User.UserID || ModuleContext.IsEditable || User.IsSuperUser))
            {
                booking.Status = "Lemondva";
                ServiceBookingManager.Instance.UpdateBooking(booking);
            }
            return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
        }
    }
}