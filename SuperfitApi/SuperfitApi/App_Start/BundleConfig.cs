using System.Web;
using System.Web.Optimization;

namespace SuperfitApi
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-3.5.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            
            bundles.Add(new ScriptBundle("~/bundles/datatablesjs").Include("~/Scripts/datatables.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatablescss").Include("~/Content/datatables.css"));
            
            bundles.Add(new StyleBundle("~/Content/fontawesome").Include("~/Content/font-awesome.css"));            

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.js",
                       "~/Scripts/bootstrap.min.js",
                       "~/Scripts/moment.min.js",
                       "~/Scripts/bootstrap-sortable.js",
                       "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-sortable.css",
                      "~/Content/Site.css"
                      ));
        }
    }
}
