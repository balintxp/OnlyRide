using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using OnlyRide.Dnn.ServiceBooking.Components;
using System.Linq;
using System.Web.Mvc;

namespace OnlyRide.Dnn.ServiceBooking.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new Models.Settings();
            settings.MaxWeeks = ModuleContext.Configuration.ModuleSettings
                .GetValueOrDefault("OnlyRide_MaxWeeks", 4);

            // Szerviz típusok betöltése a szerkesztő listához
            settings.ServiceTypes = ServiceBookingManager.Instance
                .GetServiceTypes(ModuleContext.ModuleId).ToList();

            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Models.Settings settings)
        {
            ModuleContext.Configuration.ModuleSettings["OnlyRide_MaxWeeks"] = settings.MaxWeeks.ToString();

            // Szerviz típusok becsült idejének mentése
            var serviceTypes = ServiceBookingManager.Instance
                .GetServiceTypes(ModuleContext.ModuleId);

            foreach (var st in serviceTypes)
            {
                var minutesStr = Request.Form["ServiceTypeMinutes_" + st.ServiceTypeId];
                if (int.TryParse(minutesStr, out int mins) && mins > 0)
                {
                    st.EstimatedMinutes = mins;
                    ServiceBookingManager.Instance.UpdateServiceType(st);
                }
            }

            return RedirectToDefaultRoute();
        }
    }
}