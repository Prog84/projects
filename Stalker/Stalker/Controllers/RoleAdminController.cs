using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Stalker.Areas.Administration.Controllers;
using Stalker.Entities;
using Stalker.Infrastructure;
using Stalker.Models;

namespace Stalker.Controllers
{
    [Authorize(Roles = "Administrator")]
    [MenuItem(Title = "Роли", ParentController = typeof(AdminController))]
    public class RoleAdminController : Controller
    {
        // GET: RoleAdmin
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }


        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            StalkerIdentityRole role = await RoleManager.FindByIdAsync(id);
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
            IEnumerable<StalkerIdentityUser> members =
                UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id)).OrderBy(o => o.UserName);
            IEnumerable<StalkerIdentityUser> nonMembers = UserManager.Users.Except(members).OrderBy(o => o.UserName);

            return View(new RoleEditModel
            {
                Role = role,
                DisplayName = role.DisplayName,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            StalkerIdentityRole role = await RoleManager.FindByNameAsync(model.RoleName);
            if (ModelState.IsValid)
            {
                string newDisplayName = model.DisplayName;
                role.DisplayName = newDisplayName;
                await RoleManager.UpdateAsync(role);
                IdentityResult result;
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userId,
                        model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                return RedirectToAction("Index");
            }

            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
            IEnumerable<StalkerIdentityUser> members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            IEnumerable <StalkerIdentityUser> nonMembers = UserManager.Users.Except(members);
            return View(new RoleEditModel
            {
                Role = role,
                DisplayName = role.DisplayName,
                Members = members,
                NonMembers = nonMembers
            });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(StalkerIdentityRole role)
        {
            if (role.Name == null)
            {
                ModelState.AddModelError("Name", "Заполните поле наменования роли");
            }

            if (ModelState.IsValid)
            {
                var result = RoleManager.Create(role);
                if (result == IdentityResult.Success)
                {
                    TempData[Message.SuccessMessage] = "Роль успешно создана";
                    return RedirectToAction("Index");
                }
            }

            TempData[Message.ErrorMessage] = "Не удалось создать роль";
            return View();
        }

        private StalkerUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<StalkerUserManager>();

        private StalkerRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<StalkerRoleManager>();
    }
}