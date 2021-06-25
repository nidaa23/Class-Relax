using System.Web;
using System.Web.Optimization;

namespace Class_Relax
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

           bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

           bundles.Add(new ScriptBundle("~/bundles/css").Include(
                    "~/Content/vendor/bootstrap/css/bootstrap.min.css",
               "~/Content/vendor/bootstrap/css/all.min",
                "~/Content/vendor/datatables/dataTables.bootstrap4.css",
                  "~/Content/site.css",
                     "~/Content/css/sb-admin.css"


              ));
        //    Use the development version of Modernizr to develop with and learn from. Then, when you're
        //     ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                       "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"
                     ));

           bundles.Add(new StyleBundle("~/Content/css").Include(
                          "~/Content/bootstrap-minty.css",
                           "~/Content/site.css",
                   "~/Content/css/sb-admin.css"
                   ));



            bundles.Add(new ScriptBundle("~/bundles/js").Include(

       "~/Content/vendor/jquery/jquery.min.js",
        "~/Content/vendor/bootstrap/js/bootstrap.bundle.min.js",
        "~/Content/vendor/jquery-easing/jquery.easing.min.js",
        "~/Content/js/sb-admin.js"
       ));

        }
    }
}
