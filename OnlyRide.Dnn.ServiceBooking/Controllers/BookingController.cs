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
        // Foglalás törlése - csak admin használja
        public ActionResult Delete(int bookingId)
        {
            var booking = ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);
            if (booking != null)
            {
                ServiceBookingManager.Instance.DeleteBooking(booking);
            }
            return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
        }

        // Foglalás megnyitása - új foglaláskor date paraméter jön, meglévőnél bookingId
        [HttpGet]
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

            // Meglévő foglalásnál betöltjük a jármű adatokat és ellenőrizzük szerkeszthetőséget
            // Csak holnap vagy később szerkeszthető
            if (booking.BookingId > 0)
            {
                ViewBag.Vehicle = ServiceBookingManager.Instance.GetVehicleByBooking(booking.BookingId);
                ViewBag.IsEditable = booking.CreatedOnDate.Date >= DateTime.Now.Date.AddDays(1);

                // Foglaló felhasználó adatainak betöltése
                var bookingUser = DotNetNuke.Entities.Users.UserController.GetUserById(PortalSettings.PortalId, booking.UserId);
                if (bookingUser != null)
                {
                    ViewBag.BookingUserName = bookingUser.DisplayName;
                    ViewBag.BookingUserEmail = bookingUser.Email;
                }
            }
            else
            {
                // Új foglalás mindig szerkeszthető
                ViewBag.IsEditable = true;
            }

            return View(booking);
        }

        // Foglalás mentése - új foglalásnál INSERT, meglévőnél UPDATE
        [HttpPost]
        //[ValidateAntiForgeryToken] // kikommentelve DNN kompatibilitási okból
        public ActionResult Edit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }
            try
            {
                // Admin lezárás - ha van "AdminAction" hidden mező a formban
                if (Request.Form["AdminAction"] == "complete")
                {
                    var existing = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);
                    if (existing != null && (User.IsSuperUser || ModuleContext.IsEditable))
                    {
                        existing.Status = Request.Form["Status"];
                        if (int.TryParse(Request.Form["ActualMinutes"], out int minutes))
                            existing.ActualMinutes = minutes;
                        if (decimal.TryParse(Request.Form["ActualPrice"], out decimal price))
                            existing.ActualPrice = price;
                        ServiceBookingManager.Instance.UpdateBooking(existing);
                    }
                    return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
                }

                if (booking.BookingId <= 0)
                {
                    // Új foglalás létrehozása
                    booking.UserId = User.UserID;
                    booking.ModuleId = ModuleContext.ModuleId;
                    booking.SlotId = booking.SlotId > 0 ? booking.SlotId : 0;
                    booking.ServiceTypeId = booking.ServiceTypeId > 0 ? booking.ServiceTypeId : 1;
                    booking.EstimatedPrice = 0;
                    booking.Status = "Függőben";
                    ServiceBookingManager.Instance.CreateBooking(booking);

                    // Jármű adatok mentése az új foglaláshoz
                    if (booking.BookingId > 0 && !string.IsNullOrEmpty(Request.Form["VehicleType"]))
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
                    // Meglévő foglalás frissítése
                    var existing = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);
                    if (existing != null)
                    {
                        existing.ServiceTypeId = booking.ServiceTypeId;
                        existing.CustomNote = booking.CustomNote;
                        ServiceBookingManager.Instance.UpdateBooking(existing);

                        // Jármű adatok frissítése
                        var existingVehicle = ServiceBookingManager.Instance.GetVehicleByBooking(booking.BookingId);
                        if (existingVehicle != null && !string.IsNullOrEmpty(Request.Form["VehicleType"]))
                        {
                            existingVehicle.VehicleType = Request.Form["VehicleType"];
                            existingVehicle.Brand = Request.Form["Brand"];
                            existingVehicle.Model = Request.Form["VehicleModel"];
                            existingVehicle.SerialNumber = Request.Form["SerialNumber"];
                            existingVehicle.Notes = Request.Form["VehicleNotes"];
                            ServiceBookingManager.Instance.UpdateVehicle(existingVehicle);
                        }
                    }
                }
                return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
            }
            catch (Exception ex)
            {
                return Content("MENTÉSI HIBA TÖRTÉNT: " + ex.Message);
            }
        }

        // Naptár megjelenítése - weekOffset alapján váltogatja a heteket
        public ActionResult Index(int weekOffset = 0)
        {
            var bookings = ServiceBookingManager.Instance.GetBookings(ModuleContext.ModuleId);
            ViewBag.WeekOffset = weekOffset;
            return View(bookings);
        }

        // Foglalás lemondása - státusz "Lemondva"-ra vált, csak saját vagy admin mondhatja le
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