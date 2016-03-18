using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BiBilet.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Register filters
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Register areas
            AreaRegistration.RegisterAllAreas();

            // Register routes
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Register bundles
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}