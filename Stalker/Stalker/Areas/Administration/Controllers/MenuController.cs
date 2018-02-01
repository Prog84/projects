using System.Collections.Generic;
using System.Web.Mvc;
using Stalker.Models;

namespace Stalker.Areas.Administration.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public PartialViewResult Index()
        {
            List<Menu> menus = MenuGenerator.CreateMenu();
            return PartialView("_menu", menus);
        }
    }
}