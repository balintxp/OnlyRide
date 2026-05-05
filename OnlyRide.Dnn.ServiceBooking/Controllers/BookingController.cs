using DotNetNuke.Entities.Users;
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
        /// Foglalás törlése – csak admin jogosultsággal elérhető.
        /// Törlés után visszairányít a naptár főoldalára.
        public ActionResult Delete(int bookingId)
        {
            var booking = ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);
            if (booking != null)
            {
                ServiceBookingManager.Instance.DeleteBooking(booking);
            }
            return Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID));
        }

        /// Foglalás megnyitása GET kérésre.
        /// - Ha bookingId nincs megadva (-1 vagy 0): új foglalás létrehozása a megadott dátummal
        /// - Ha bookingId meg van adva: meglévő foglalás betöltése szerkesztésre
        [HttpGet]
        public ActionResult Edit(int bookingId = -1, string date = "")
        {
            // Szerviz típusok betöltése a legördülő menühöz
            ViewBag.ServiceTypes = ServiceBookingManager.Instance.GetServiceTypes(ModuleContext.ModuleId);

            // Új vagy meglévő foglalás meghatározása
            var booking = (bookingId == -1 || bookingId == 0)
                ? new Booking
                {
                    ModuleId = ModuleContext.ModuleId,
                    CreatedOnDate = !string.IsNullOrEmpty(date) ? DateTime.Parse(date) : DateTime.Now,
                    Status = "Függőben"
                }
                : ServiceBookingManager.Instance.GetBooking(bookingId, ModuleContext.ModuleId);

            // Meglévő foglalásnál extra adatok betöltése
            if (booking.BookingId > 0)
            {
                // Jármű adatok betöltése a nézethez
                ViewBag.Vehicle = ServiceBookingManager.Instance.GetVehicleByBooking(booking.BookingId);

                // Csak holnaptól módosítható a foglalás (ma már nem)
                ViewBag.IsEditable = booking.CreatedOnDate.Date >= DateTime.Now.Date.AddDays(1);

                // Foglaló felhasználó nevének és emailjének betöltése (admin nézethez)
                var bookingUser = UserController.GetUserById(PortalSettings.PortalId, booking.UserId);
                if (bookingUser != null)
                {
                    ViewBag.BookingUserName = bookingUser.DisplayName;
                    ViewBag.BookingUserEmail = bookingUser.Email;
                }
            }
            else
            {
                // Új foglalás esetén mindig szerkeszthető
                ViewBag.IsEditable = true;
            }

            return View(booking);
        }

        /// Foglalás mentése POST kérésre. Háromféle eset lehetséges:
        /// 1. Admin lezárja a szervizt (IsAdminForm = true)
        /// 2. Új foglalás leadása normál felhasználótól
        /// 3. Meglévő foglalás szerkesztése normál felhasználótól
        [HttpPost]
        // [ValidateAntiForgeryToken] – kikommentelve, DNN MVC kompatibilitási okok miatt
        public ActionResult Edit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            try
            {
                // 1. ESET: ADMIN LEZÁRJA A SZERVIZT
                // Az admin űrlapon egy rejtett "IsAdminForm" mező jelzi,
                // hogy ez adminisztrátori művelet, nem normál felhasználói.
                if (Request.Form["IsAdminForm"] == "true" && (User.IsSuperUser || ModuleContext.IsEditable))
                {
                    // Friss adatot töltünk az adatbázisból, hogy ne dolgozzunk elavult adattal
                    var existing = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);

                    if (existing != null)
                    {
                        existing.Status = booking.Status;

                        // Ha az admin megadta a tényleges időt, azt mentjük,
                        // egyébként a szerviz típus alapértelmezett idejét használjuk
                        if (booking.ActualMinutes.HasValue)
                        {
                            existing.ActualMinutes = booking.ActualMinutes;
                        }
                        else
                        {
                            var st = ServiceBookingManager.Instance.GetServiceType(existing.ServiceTypeId, ModuleContext.ModuleId);
                            if (st != null) existing.ActualMinutes = st.EstimatedMinutes;
                        }

                        // Ha az admin megadta a tényleges árat, azt mentjük,
                        // egyébként a szerviz típus alapárát használjuk
                        if (booking.ActualPrice.HasValue)
                        {
                            existing.ActualPrice = booking.ActualPrice;
                        }
                        else
                        {
                            var st = ServiceBookingManager.Instance.GetServiceType(existing.ServiceTypeId, ModuleContext.ModuleId);
                            if (st != null) existing.ActualPrice = st.BasePrice;
                        }

                        // Lezárás mentése az adatbázisba
                        ServiceBookingManager.Instance.UpdateBooking(existing);

                        // Ha a státusz "Kész" és van tényleges ár, Hotcakes rendelést hozunk létre
                        if (existing.Status != null &&
                            existing.Status.Trim().Equals("Kész", StringComparison.OrdinalIgnoreCase) &&
                            existing.ActualPrice.HasValue)
                        {
                            try
                            {
                                var serviceType = ServiceBookingManager.Instance.GetServiceType(existing.ServiceTypeId, ModuleContext.ModuleId);
                                var dnnUser = UserController.GetUserById(PortalSettings.PortalId, existing.UserId);

                                string userEmail = dnnUser?.Email ?? "ismeretlen@onlyride.hu";
                                string displayName = !string.IsNullOrEmpty(dnnUser?.DisplayName)
                                    ? dnnUser.DisplayName
                                    : "Ügyfelünk";

                                // Hotcakes belső API inicializálása
                                // (REST API helyett ezt használjuk, mert az nem futtatja le
                                // a teljes workflow-t: nem generál rendelésszámot és Cancelled státuszt ad)
                                var hccApp = HotcakesApplication.Current;

                                if (hccApp != null)
                                {
                                    // Hotcakes rendelés összeállítása
                                    var order = new Order();
                                    order.UserEmail = userEmail;
                                    order.BillingAddress.FirstName = dnnUser?.FirstName ?? "";
                                    order.BillingAddress.LastName = dnnUser?.LastName ?? "";
                                    order.BillingAddress.Line1 = "-";
                                    order.BillingAddress.City = "Budapest";
                                    order.BillingAddress.PostalCode = "1000";

                                    order.ShippingAddress.FirstName = dnnUser?.FirstName ?? "";
                                    order.ShippingAddress.LastName = dnnUser?.LastName ?? "";
                                    order.ShippingAddress.Line1 = "-";
                                    order.ShippingAddress.City = "Budapest";
                                    order.ShippingAddress.PostalCode = "1000";

                                    // Szerviz tétel hozzáadása a rendeléshez
                                    var lineItem = new LineItem();
                                    lineItem.ProductName = serviceType?.Name ?? "Szerviz";
                                    lineItem.ProductSku = "SZERVIZ-" + existing.ServiceTypeId;
                                    lineItem.BasePricePerItem = existing.ActualPrice.Value;
                                    lineItem.AdjustedPricePerItem = existing.ActualPrice.Value;
                                    lineItem.LineTotal = existing.ActualPrice.Value;
                                    lineItem.Quantity = 1;
                                    order.Items.Add(lineItem);

                                    // Rendelés mentése az adatbázisba
                                    hccApp.OrderServices.Orders.Create(order);

                                    // Rendelésszám generálása a Hotcakes store-hoz kötve
                                    long orderNumber = hccApp.OrderServices.GenerateNewOrderNumber(hccApp.CurrentStore.Id);
                                    order.OrderNumber = orderNumber.ToString();

                                    // Státusz "Received"-re állítása (alapból Cancelled lenne)
                                    order.StatusCode = OrderStatusCode.Received;
                                    order.StatusName = "Received";

                                    // Frissített rendelés visszamentése
                                    hccApp.OrderServices.Orders.Update(order);

                                    // Értesítő email küldése az ügyfélnek a rendelésszámmal
                                    // (Hotcakes ContentServices-en keresztül nem volt elérhető
                                    // közvetlen email küldő metódus, ezért DNN Mail API-t használunk)
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
                                            displayName,
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


                // 2. ESET: ÚJ FOGLALÁS LEADÁSA (normál felhasználó)
                if (booking.BookingId <= 0)
                {
                    // Jármű adatok kiolvasása az űrlapból (külön form mezők, nem a Booking modellben vannak)
                    string vehicleType = Request.Form["VehicleType"];
                    string brand = Request.Form["Brand"];
                    string vehicleModel = Request.Form["VehicleModel"];
                    string serialNumber = Request.Form["SerialNumber"];
                    string vehicleNotes = Request.Form["VehicleNotes"];

                    // Foglalás adatok beállítása
                    booking.UserId = User.UserID;
                    booking.ModuleId = ModuleContext.ModuleId;
                    booking.SlotId = booking.SlotId > 0 ? booking.SlotId : 0;
                    booking.ServiceTypeId = booking.ServiceTypeId > 0 ? booking.ServiceTypeId : 1;
                    booking.EstimatedPrice = 0;
                    booking.Status = "Függőben";

                    ServiceBookingManager.Instance.CreateBooking(booking);

                    // Jármű adatok mentése – csak ha a foglalás sikeresen létrejött és van jármű típus
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

                    // Visszaigazoló email küldése a foglalónak
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

                // 3. ESET: MEGLÉVŐ FOGLALÁS SZERKESZTÉSE (normál felhasználó)
                else
                {
                    var existingUserBooking = ServiceBookingManager.Instance.GetBooking(booking.BookingId, ModuleContext.ModuleId);
                    if (existingUserBooking != null)
                    {
                        // Csak a szerviz típus és a megjegyzés módosítható felhasználó által
                        existingUserBooking.ServiceTypeId = booking.ServiceTypeId;
                        existingUserBooking.CustomNote = booking.CustomNote;
                        ServiceBookingManager.Instance.UpdateBooking(existingUserBooking);

                        // Jármű adatok frissítése ha meg lettek adva
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


        /// Naptár főoldal megjelenítése.
        /// - Hotcakes termékek szinkronizálása a ServiceTypes táblába (SZERVIZ- SKU alapján)
        /// - weekOffset alapján váltja a heti nézetet (0 = aktuális hét)
        public ActionResult Index(int weekOffset = 0)
        {
            // Hotcakes szinkronizáció: minden oldalbetöltésnél ellenőrizzük,
            // hogy van-e új SZERVIZ- előtagú termék a Hotcakesben amit még nem vettünk fel
            var hccApp = HotcakesApplication.Current;
            if (hccApp != null)
            {
                // Összes termék lekérése, majd SZERVIZ- előtagú SKU-k szűrése
                var allProducts = hccApp.CatalogServices.Products.FindAllPaged(1, 999);
                var serviceProducts = allProducts.Where(p => p.Sku.StartsWith("SZERVIZ-")).ToList();

                foreach (var product in serviceProducts)
                {
                    // Megnézzük, van-e már ilyen nevű szerviz típus az adatbázisban
                    var existing = ServiceBookingManager.Instance
                        .GetServiceTypes(ModuleContext.ModuleId)
                        .FirstOrDefault(s => s.Name == product.ProductName);

                    if (existing == null)
                    {
                        // Új szerviz típus létrehozása a Hotcakes termék adatai alapján
                        ServiceBookingManager.Instance.CreateServiceType(new ServiceType
                        {
                            Name = product.ProductName,
                            Description = product.LongDescription,
                            BasePrice = product.SitePrice,
                            EstimatedMinutes = 60, // Alapértelmezett becsült idő
                            IsActive = true,
                            ModuleId = ModuleContext.ModuleId
                        });
                    }
                    else
                    {
                        // Meglévő szerviz típus árának frissítése ha változott a Hotcakesben
                        existing.BasePrice = product.SitePrice;
                        ServiceBookingManager.Instance.UpdateServiceType(existing);
                    }
                }
            }

            var bookings = ServiceBookingManager.Instance.GetBookings(ModuleContext.ModuleId);
            ViewBag.WeekOffset = weekOffset;
            return View(bookings);
        }


        /// Foglalás lemondása – csak a saját foglalását mondhatja le a felhasználó,
        /// vagy admin/superuser bármelyiket.

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