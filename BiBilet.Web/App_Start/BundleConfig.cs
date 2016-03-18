using System.Web.Optimization;

namespace BiBilet.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/assets/css/app")
                .Include("~/assets/css/site.css"));

            bundles.Add(new ScriptBundle("~/assets/js/app")
                .Include("~/assets/js/jquery-{version}.js")
                .Include("~/assets/js/jquery.validate*")
                .Include("~/assets/js/bootstrap.js")
                .Include("~/assets/js/tinymce/tinymce.min.js")
                .Include("~/assets/js/site.js"));

            // Enables versioning & minifying ( Web.Config override )
            //BundleTable.EnableOptimizations = true;
        }
    }
}