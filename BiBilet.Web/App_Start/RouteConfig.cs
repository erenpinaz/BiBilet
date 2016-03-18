using System.Web.Mvc;
using System.Web.Routing;

namespace BiBilet.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Force lowercase URLs
            routes.LowercaseUrls = true;

            // Route: Event Details
            routes.MapRoute(
                "Event_Details",
                "event/{slug}",
                new {controller = "Event", action = "Details"});

            // Route: Default
            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new {controller = "Home", action = "Index"},
                new[] {"BiBilet.Web.Controllers"}).DataTokens["UseNamespaceFallback"] = false;
        }
    }
}