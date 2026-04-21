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
            return RedirectToDefaultRoute();
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

                return Redirect(DotNetNuke.Common.Globals.NavigateURL());
            }
            catch (Exception ex)
            {
                return Content("MENTÉSI HIBA TÖRTÉNT: " + ex.Message);
            }
        }
        public ActionResult Index()
        {
            var bookings = ServiceBookingManager.Instance.GetBookings(ModuleContext.ModuleId);
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
            return Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }
    }
}