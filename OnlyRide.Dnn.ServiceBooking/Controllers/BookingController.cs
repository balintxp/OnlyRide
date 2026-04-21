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

using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
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

        public ActionResult Edit(int bookingId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            // Ez betölti a felhasználókat egy legördülő menühöz (opcionális, de meghagyjuk)
            var userlist = UserController.GetUsers(PortalSettings.PortalId);
            var users = from user in userlist.Cast<UserInfo>().ToList()
                        select new SelectListItem { Text = user.DisplayName, Value = user.UserID.ToString() };

            ViewBag.Users = users;

            var booking = (bookingId == -1 || bookingId == 0)
                 ? new Booking { ModuleId = ModuleContext.ModuleId }
                 : ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);

            return View(booking);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(Booking booking)
        {
            // Ha a BookingId 0 vagy -1, akkor ez egy új foglalás
            if (booking.BookingId == -1 || booking.BookingId == 0)
            {
                booking.UserId = User.UserID; // Az éppen bejelentkezett felhasználó ID-ja
                booking.CreatedOnDate = DateTime.UtcNow; // UTC időt használunk a PDF utasítása alapján
                booking.ModuleId = ModuleContext.ModuleId;

                ServiceBookingManager.Instance.CreateBooking(booking);
            }
            else
            {
                // Meglévő foglalás frissítése
                var existingBooking = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);
                if (existingBooking != null)
                {
                    existingBooking.SlotId = booking.SlotId;
                    existingBooking.ServiceTypeId = booking.ServiceTypeId;
                    existingBooking.CustomNote = booking.CustomNote;
                    existingBooking.Status = booking.Status;
                    // A szerelő által kitöltött mezők:
                    existingBooking.ActualMinutes = booking.ActualMinutes;
                    existingBooking.ActualPrice = booking.ActualPrice;

                    ServiceBookingManager.Instance.UpdateBooking(existingBooking);
                }
            }

            return RedirectToDefaultRoute();
        }

        [ModuleAction(ControlKey = "Edit", TitleKey = "AddBooking")]
        public ActionResult Index()
        {
            // Főoldal: kilistázzuk a foglalásokat
            var bookings = ServiceBookingManager.Instance.GetBookings(ModuleContext.ModuleId);
            return View(bookings);
        }
    }
}