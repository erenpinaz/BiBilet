using System.Web.Optimization;

namespace BiBilet.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/assets/css/app")
                .Include("~/assets/css/site.css")
                .Include("~/assets/css/bootstrap-datetimepicker.css")
                .Include("~/assets/css/bootstrap-table.css"));

            bundles.Add(new ScriptBundle("~/assets/js/app")
                .Include("~/assets/js/jquery-{version}.js")
                .Include("~/assets/js/jquery.validate*")
                .Include("~/assets/js/jquery.form.js")
                .Include("~/assets/js/jquery.cropit.js")
                .Include("~/assets/js/moment.js")
                .Include("~/assets/js/bootstrap.js")
                .Include("~/assets/js/bootstrap-datetimepicker.js")
                .Include("~/assets/js/bootstrap-table/bootstrap-table.js")
                .Include("~/assets/js/bootstrap-table/extensions/cookie/bootstrap-table-cookie.js")
                .Include("~/assets/js/bootstrap-table/locale/bootstrap-table-tr-TR.js")
                .Include("~/assets/js/site.js"));

            bundles.Add(new ScriptBundle("~/assets/js/plupload/plupload.full.min.js")
                .Include("~/assets/js/plupload/plupload.full.min.js"));

            bundles.Add(new ScriptBundle("~/assets/js/tinymce/tinymce.min.js")
                .Include("~/assets/js/tinymce/tinymce.min.js"));

            // Enables versioning & minifying ( Web.Config override )
            //BundleTable.EnableOptimizations = true;
        }
    }
}