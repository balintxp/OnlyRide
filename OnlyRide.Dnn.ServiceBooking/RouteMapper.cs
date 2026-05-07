using DotNetNuke.Web.Api;

namespace OnlyRide.Dnn.ServiceBooking
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(DotNetNuke.Web.Api.IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "OnlyRide.Dnn.ServiceBooking",
                routeName: "default",
                url: "{controller}/{action}",
                defaults: new { },
                namespaces: new[] { "OnlyRide.Dnn.ServiceBooking.Controllers" }
            );
        }
    }
}