using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Stalker
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterStyleBundles(bundles);
            RegisterScriptsBundles(bundles);
        }

        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                .Include("~/Content/bootstrap-theme.min.css")
                .Include("~/Content/Bootstrap.min.css")
                //.Include("~/Content/bootstrap-responsive.css")
                .Include("~/Content/font-awesome.css")
                .Include("~/Content/Cite.css")
                .Include("~/Content/themes/base/jquery-ui.min.css")
                .Include("~/Content/themes/base/datepicker.css")
                .Include("~/Content/tableStyles.css")
                .Include("~/Content/ErrorStyles.css"));
        }

        private static void RegisterScriptsBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/jquery-ui-{version}.js")
                .Include("~/scripts/jquery.unobtrusive-ajax.js")
                .Include("~/scripts/bootstrap.js")
                .Include("~/scripts/jQuery-ui-datepicker-ru.js")
                .Include("~/Areas/Admin/Scripts/AjaxSendingData.js"));
        }
    }
}