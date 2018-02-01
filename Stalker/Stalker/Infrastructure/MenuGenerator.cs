using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stalker.Models;
using System.Reflection;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Stalker.Infrastructure;


namespace Stalker
{
    public static class MenuGenerator
    {
        private static StalkerUserManager UserManager => HttpContext.Current.GetOwinContext().GetUserManager<StalkerUserManager>();
        public static List<Menu> CreateMenu()
        {
            var menus = new List<Menu>();

            var currentAssembly = Assembly.GetAssembly(typeof(MenuGenerator));
            var allControllers = currentAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Controller))).ToList();
            var menuControllers = allControllers.Where(t => t.GetCustomAttribute<MenuItemAttribute>() != null ||
                                                             t.GetMethods().Any(m => m.GetCustomAttribute<MenuItemAttribute>() != null))
                                                             .ToList();
            var submenuControllers = new List<Menu>();
            menuControllers.ForEach(controller =>
            {
                var navigation = controller.GetCustomAttribute<MenuItemAttribute>();
                if (navigation == null) //навигация только на методах действий
                {
                    controller.GetMethods().ToList().ForEach(method =>
                    {
                        navigation = method.GetCustomAttribute<MenuItemAttribute>();
                        if (navigation == null) return;
                        if (!UserHasAccess(method.GetCustomAttribute<AuthorizeAttribute>())) return;
                        Menu actionMenu = CreateAreaMenuItemFromAction(controller, method, navigation);
                        menus.Add(actionMenu);
                    });
                    return;
                }

                if (!UserHasAccess(controller.GetCustomAttribute<AuthorizeAttribute>())) return;
                Menu menu = CreateAreaMenuItemFromController(controller, navigation);
                if (navigation.ParentController != null)
                {
                    if (navigation.ParentController.IsSubclassOf(typeof(Controller)))
                    {
                        menu.ParentControllerFullName = navigation.ParentController.FullName;
                        submenuControllers.Add(menu);
                    }
                }
                menus.Add(menu);
            });
            menus = menus.Except(submenuControllers).ToList();
            submenuControllers.ForEach(sm =>
            {
                var parentMenu = menus.FirstOrDefault(m => m.ControllerFullName == sm.ParentControllerFullName);
                parentMenu?.SubMenus.Add(new SubMenu() { Name = sm.Name, Url = sm.Url });
            });
            return menus.OrderBy(m => m.Order).ToList();
        }



        private static Menu CreateAreaMenuItemFromController(Type controller, MenuItemAttribute menuItemAttribute)
        {
            string area = GetAreaNameForController(controller);
            var controllerName = controller.Name.Replace("Controller", "");
            var menu = new Menu()
            {
                Name = menuItemAttribute.Title ?? controllerName,
                ControllerFullName = controller.FullName,
                Order = menuItemAttribute.Order,
                CssIcon = menuItemAttribute.CssIcon
            };

            if (menuItemAttribute.IsClickable)
            {
                menu.Url = CreateActionPath(area, controllerName, menuItemAttribute.Action ?? "Index");
            }
            var submenus = new List<SubMenu>(); //The actions under the controller becomes submenu
            controller.GetMethods().ToList().ForEach(method =>
            {
                menuItemAttribute = method.GetCustomAttribute<MenuItemAttribute>();
                if (menuItemAttribute == null) return;
                if (!UserHasAccess(method.GetCustomAttribute<AuthorizeAttribute>())) return;
                var submenu = new SubMenu()
                {
                    Name = menuItemAttribute.Title ?? method.Name,
                    Order = menuItemAttribute.Order,
                    CssIcon = menuItemAttribute.CssIcon

                };
                if (menuItemAttribute.IsClickable)
                {
                    submenu.Url = CreateActionPath(area, controllerName, method.Name);
                }
                submenus.Add(submenu);
            });
            menu.SubMenus = submenus.OrderBy(m => m.Order).ToList();
            return menu;
        }

        private static bool UserHasAccess(AuthorizeAttribute authorizeAttribute)
        {
            if (authorizeAttribute == null)
                return true;
            string[] rolesWithoutSpacesMas = authorizeAttribute.Roles.Replace(" ", "").Split(',');
            return rolesWithoutSpacesMas.Any(role => HttpContext.Current.User.IsInRole(role));
        }

        private static string CreateActionPath(string area, string controller, string action)
        {
            if (string.IsNullOrWhiteSpace(area))
                return $"~/{controller}/{action}";
            return $"~/{area}/{controller}/{action}";
        }

        private static Menu CreateAreaMenuItemFromAction(Type controller, MethodInfo method, MenuItemAttribute menuItemAttribute)
        {

            string area = GetAreaNameForController(controller);

            var menu = new Menu()
            {
                Name = menuItemAttribute.Title ?? method.Name,
                ControllerFullName = controller.FullName,
                Order = menuItemAttribute.Order,
                CssIcon = menuItemAttribute.CssIcon
            };
            if (menuItemAttribute.IsClickable)
            {
                menu.Url = CreateActionPath(area, controller.Name.Replace("Controller", ""), method.Name);
            }
            return menu;
        }

        private static string GetAreaNameForController(Type controller)
        {
            var area = "";
            if (string.IsNullOrWhiteSpace(controller.Namespace)) return area;
            if (controller.Namespace.Contains("Areas"))
            {
                var parts = controller.Namespace.Split('.').ToList();
                area = parts[parts.FindLastIndex(n => n.Equals("Areas")) + 1];
            }
            return area;
        }
    }
}