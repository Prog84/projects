using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using Stalker.Infrastructure;

namespace Stalker.Controllers
{
    public class HomeController : Controller
    {
        //[MenuItem(Title = "Домой", Order = 500, CssIcon = "fa fa-home fa-lg fa-fw")]
        public ActionResult Index()
        {
            var res = View("Index", new { area = "" });
            return res;
        }
    }
}