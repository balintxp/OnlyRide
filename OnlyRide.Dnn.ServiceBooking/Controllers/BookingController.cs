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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Orders;

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
                // 1. ADMIN LEZÁRÁS (Ha a rejtett mező "true")
                if (Request.Form["IsAdminForm"] == "true" && (User.IsSuperUser || ModuleContext.IsEditable))
                {
                    // ITT TÖLTJÜK BE AZ 'existing' VÁLTOZÓT AZ ADATBÁZISBÓL!
                    var existing = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);

                    if (existing != null)
                    {
                        existing.Status = booking.Status;

                        // Fallback logika az időre
                        if (booking.ActualMinutes.HasValue)
                        {
                            existing.ActualMinutes = booking.ActualMinutes;
                        }
                        else
                        {
                            var st = ServiceBookingManager.Instance.GetServiceType(existing.ServiceTypeId, ModuleContext.ModuleId);
                            if (st != null) existing.ActualMinutes = st.EstimatedMinutes;
                        }

                        // Fallback logika az árra
                        if (booking.ActualPrice.HasValue)
                        {
                            existing.ActualPrice = booking.ActualPrice;
                        }
                        else
                        {
                            var st = ServiceBookingManager.Instance.GetServiceType(existing.ServiceTypeId, ModuleContext.ModuleId);
                            if (st != null) existing.ActualPrice = st.BasePrice;
                        }

                        // Elmentjük a DNN adatbázisba a lezárást
                        ServiceBookingManager.Instance.UpdateBooking(existing);

                        // Debug naplózás, hogy lásd, mi van benne pontosan
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(new Exception("DEBUG - Status: [" + existing.Status + "], Price: " + existing.ActualPrice));

                        // Robusztusabb ellenőrzés
                        bool isDone = string.Equals(existing.Status?.Trim(), "Kész", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(existing.Status?.Trim(), "Kesz", StringComparison.OrdinalIgnoreCase);

                        // HOTCAKES RENDELÉS LÉTREHOZÁSA
                        if (existing.Status != null && existing.Status.Trim().Equals("Kész", StringComparison.OrdinalIgnoreCase) && existing.ActualPrice.HasValue)
                        {
                            try
                            {
                                var serviceType = ServiceBookingManager.Instance.GetServiceType(existing.ServiceTypeId, ModuleContext.ModuleId);
                                var dnnUser = DotNetNuke.Entities.Users.UserController.GetUserById(PortalSettings.PortalId, existing.UserId);

                                string userEmail = dnnUser?.Email ?? "ismeretlen@onlyride.hu";
                                string firstName = dnnUser?.FirstName ?? "OnlyRide";
                                string lastName = dnnUser?.LastName ?? "Ügyfél";

                                // Hotcakes alkalmazás inicializálása
                                var hccApp = Hotcakes.Commerce.HotcakesApplication.Current;

                                // Debug: null-e?
                                DotNetNuke.Services.Exceptions.Exceptions.LogException(
                                    new Exception("hccApp null-e: " + (hccApp == null ? "IGEN - NULL!" : "Nem null, OK")));

                                if (hccApp == null)
                                {
                                    DotNetNuke.Services.Exceptions.Exceptions.LogException(
                                        new Exception("Hotcakes Current null - nem lehet rendelést létrehozni"));
                                    // Fallback: REST API-val próbálkozunk
                                }
                                else
                                {
                                    // ... a rendelés létrehozó kód
                                    // Új rendelés létrehozása
                                    var order = new Hotcakes.Commerce.Orders.Order();
                                    order.UserEmail = userEmail;
                                    order.BillingAddress.FirstName = firstName;
                                    order.BillingAddress.LastName = lastName;
                                    order.BillingAddress.Line1 = "-";
                                    order.BillingAddress.City = "Budapest";
                                    order.BillingAddress.PostalCode = "1000";
                                    // CountryBvin-t NE add meg - hagyd üresen, a Hotcakes maga tölti fel

                                    order.ShippingAddress.FirstName = firstName;
                                    order.ShippingAddress.LastName = lastName;
                                    order.ShippingAddress.Line1 = "-";
                                    order.ShippingAddress.City = "Budapest";
                                    order.ShippingAddress.PostalCode = "1000";

                                    // Tétel hozzáadása
                                    var lineItem = new Hotcakes.Commerce.Orders.LineItem();
                                    lineItem.ProductName = serviceType?.Name ?? "Szerviz";
                                    lineItem.ProductSku = "SZERVIZ-" + existing.ServiceTypeId;
                                    lineItem.BasePricePerItem = existing.ActualPrice.Value;
                                    lineItem.AdjustedPricePerItem = existing.ActualPrice.Value;
                                    lineItem.LineTotal = existing.ActualPrice.Value;
                                    lineItem.Quantity = 1;
                                    order.Items.Add(lineItem);

                                    hccApp.OrderServices.Orders.Create(order);

                                    long orderNumber = hccApp.OrderServices.GenerateNewOrderNumber(hccApp.CurrentStore.Id);
                                    order.OrderNumber = orderNumber.ToString();

                                    // Státusz beállítása Received-re (ne legyen Cancelled)
                                    order.StatusCode = Hotcakes.Commerce.Orders.OrderStatusCode.Received;
                                    order.StatusName = "Received";

                                    hccApp.OrderServices.Orders.Update(order);

                                    // Email küldése az ügyfélnek DNN-en keresztül
                                    DotNetNuke.Services.Mail.Mail.SendMail(
                                        PortalSettings.Email,
                                        userEmail,
                                        "",
                                        "OnlyRide – Szerviz elkészült | Rendelésszám: #" + order.OrderNumber,
                                        string.Format(
                                            "Kedves {0}!<br><br>" +
                                            "A szervizünkben elvégzett munkálatok befejeződtek.<br><br>" +
                                            "<b>Rendelés adatai:</b><br>" +
                                            "Rendelésszám: #{1}<br>" +
                                            "Szolgáltatás: {2}<br>" +
                                            "Fizetendő összeg: {3} Ft<br><br>" +
                                            "Kérjük, jöjjön el a járműért és fizesse ki a szerviz díját.<br><br>" +
                                            "Üdvözlettel,<br>OnlyRide Szerviz",
                                            firstName,
                                            order.OrderNumber,
                                            serviceType?.Name ?? "-",
                                            existing.ActualPrice?.ToString() ?? "-"
                                        ),
                                        "", "HTML", "", "", "", ""
                                    );
                                }

                                
                            }
                            catch (Exception ex)
                            {
                                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                            }
                        }
                    }
                        return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
                }

                // 2. NORMÁL USER: Új foglalás leadása
                if (booking.BookingId <= 0)
                {
                    string vehicleType = Request.Form["VehicleType"];
                    string brand = Request.Form["Brand"];
                    string vehicleModel = Request.Form["VehicleModel"];
                    string serialNumber = Request.Form["SerialNumber"];
                    string vehicleNotes = Request.Form["VehicleNotes"];

                    booking.UserId = User.UserID;
                    booking.ModuleId = ModuleContext.ModuleId;
                    booking.SlotId = booking.SlotId > 0 ? booking.SlotId : 0;
                    booking.ServiceTypeId = booking.ServiceTypeId > 0 ? booking.ServiceTypeId : 1;
                    booking.EstimatedPrice = 0;
                    booking.Status = "Függőben";

                    ServiceBookingManager.Instance.CreateBooking(booking);

                    // Jármű adatok mentése
                    if (booking.BookingId > 0 && !string.IsNullOrEmpty(vehicleType))
                    {
                        var vehicle = new Vehicle
                        {
                            BookingId = booking.BookingId,
                            VehicleType = vehicleType,
                            Brand = brand,
                            Model = vehicleModel,
                            SerialNumber = serialNumber,
                            Notes = vehicleNotes
                        };
                        ServiceBookingManager.Instance.CreateVehicle(vehicle);
                    }

                    // DNN Email küldése a foglalásról
                    var serviceTypes = ServiceBookingManager.Instance.GetServiceTypes(ModuleContext.ModuleId);
                    var selectedService = serviceTypes.FirstOrDefault(s => s.ServiceTypeId == booking.ServiceTypeId);

                    string emailBody = string.Format(
                        "Kedves {0}!<br><br>" +
                        "Sikeresen foglalt időpontot az OnlyRide szervizbe.<br><br>" +
                        "<b>Foglalás adatai:</b><br>" +
                        "Időpont: {1}<br>" +
                        "Szolgáltatás: {2}<br>" +
                        "Becsült ár: {3} Ft<br>" +
                        "Becsült idő: {4} perc<br><br>" +
                        "Jármű adatai:<br>" +
                        "Típus: {5}<br>" +
                        "Márka / Modell: {6} {7}<br>" +
                        "Sorozatszám: {8}<br><br>" +
                        "Hiba leírása: {9}<br><br>" +
                        "Üdvözlettel,<br>OnlyRide Szerviz",
                        User.DisplayName,
                        booking.CreatedOnDate.ToString("yyyy. MM. dd. HH:mm"),
                        selectedService != null ? selectedService.Name : "-",
                        selectedService != null ? selectedService.BasePrice.ToString() : "-",
                        selectedService != null ? selectedService.EstimatedMinutes.ToString() : "-",
                        vehicleType,
                        brand,
                        vehicleModel,
                        serialNumber,
                        booking.CustomNote ?? "-"
                    );

                    DotNetNuke.Services.Mail.Mail.SendMail(
                        PortalSettings.Email,
                        User.Email,
                        "",
                        "OnlyRide – Foglalás visszaigazolása",
                        emailBody,
                        "",
                        "HTML",
                        "",
                        "",
                        "",
                        ""
                    );
                }
                // 3. NORMÁL USER: Meglévő foglalásának szerkesztése
                else
                {
                    var existingUserBooking = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);
                    if (existingUserBooking != null)
                    {
                        existingUserBooking.ServiceTypeId = booking.ServiceTypeId;
                        existingUserBooking.CustomNote = booking.CustomNote;
                        ServiceBookingManager.Instance.UpdateBooking(existingUserBooking);

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
                return Content("MENTÉSI HIBA TÖRTÉNT: " + ex.Message + " | " + ex.StackTrace);
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