using DotNetNuke.Web.Api;
using OnlyRide.Dnn.ServiceBooking.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace OnlyRide.Dnn.ServiceBooking.Controllers
{
    // Elérhető: /API/OnlyRide.Dnn.ServiceBooking/BookingApi/GetBookingsByServiceType?moduleId=X
    public class BookingApiController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetBookingsByServiceType(int moduleId)
        {
            var bookings = ServiceBookingManager.Instance.GetBookings(moduleId);
            var serviceTypes = ServiceBookingManager.Instance.GetServiceTypes(moduleId);

            var result = serviceTypes.Select(st => new
            {
                ServiceTypeId = st.ServiceTypeId,
                ServiceTypeName = st.Name,
                BasePrice = st.BasePrice,
                EstimatedMinutes = st.EstimatedMinutes,
                BookingCount = bookings.Count(b => b.ServiceTypeId == st.ServiceTypeId && b.Status != "Lemondva"),
                Bookings = bookings
                    .Where(b => b.ServiceTypeId == st.ServiceTypeId && b.Status != "Lemondva")
                    .Select(b => new
                    {
                        b.BookingId,
                        b.Status,
                        b.CreatedOnDate,
                        b.ActualPrice,
                        b.ActualMinutes,
                        b.CustomNote
                    })
                    .OrderBy(b => b.CreatedOnDate)
                    .ToList()
            }).ToList();

            return Json(result); // Ok() helyett Json()
        }
    }
}