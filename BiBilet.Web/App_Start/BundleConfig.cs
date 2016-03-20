using System.Web.Optimization;

namespace BiBilet.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/assets/css/app")
                .Include("~/assets/css/site.css")
                .Include("~/assets/css/bootstrap-table.css"));

            bundles.Add(new ScriptBundle("~/assets/js/app")
                .Include("~/assets/js/jquery-{version}.js")
                .Include("~/assets/js/jquery.validate*")
                .Include("~/assets/js/bootstrap.js")
                .Include("~/assets/js/bootstrap-table/bootstrap-table.js")
                .Include("~/assets/js/bootstrap-table/locale/bootstrap-table-tr-TR.js")
                .Include("~/assets/js/plupload/moxie.js")
                .Include("~/assets/js/plupload/plupload.js")
                .Include("~/assets/js/site.js"));

            bundles.Add(new ScriptBundle("~/assets/js/tinymce/tinymce.min.js")
                .Include("~/assets/js/tinymce/tinymce.min.js"));

            // Enables versioning & minifying ( Web.Config override )
            BundleTable.EnableOptimizations = true;
        }
    }
}