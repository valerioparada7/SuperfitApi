using System.Web;
using System.Web.Optimization;

namespace SuperfitApi
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));*/
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.5.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.js",
                       "~/Scripts/bootstrap.min.js",
                       "~/Scripts/moment.min.js",
                       "~/Scripts/bootstrap-sortable.js",
                       "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatablesjs").Include(
                  "~/Scripts/datatables.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatablescss").Include(
                        "~/Content/datatables.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-sortable.css",
                      "~/Content/Site.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                       "~/Content/font-awesome.css"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                        "~/Scripts/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                        "~/Scripts/Chart.min.js"));

            //bundles.Add(new StyleBundle("~/Content/sweetalert").Include(
            //            "~/Content/sweetalert.css"));

            bundles.Add(new ScriptBundle("~/bundles/chosen").Include("~/Scripts/chosen.jquery*"));
            bundles.Add(new ScriptBundle("~/Content/chosen").Include("~/Scripts/bootstrap-chosen.css"));
        }
    }
}
