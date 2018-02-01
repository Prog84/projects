using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Stalker.Entities;
using Stalker.Infrastructure;
using Stalker.Models;


namespace Stalker.Areas.Administration.Controllers
{
    //Этот контроллер отвечает за аутентификацию пользователя
    [Authorize]
    public class AccountController : Controller
    {
        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

        private StalkerUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<StalkerUserManager>();

        [AllowAnonymous]
        public  ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                StalkerIdentityUser user = await UserManager.FindAsync(details.Login, details.Password);
                if (user == null)
                {
                    string errorMessage = "Логин или пароль не распознаны";
                    ModelState.AddModelError("", errorMessage);
                }
                else
                {
                    if (user.LockoutEnabled)
                    {
                        string errorMessage = "Пользователь заблокирован";
                        ModelState.AddModelError("", errorMessage);
                        ViewBag.returnUrl = returnUrl;
                        return View();
                    }

                    ClaimsIdentity ident =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public ActionResult Logout()
        {
            AuthManager.SignOut();
            RedirectToRouteResult redirect = RedirectToAction("Index", new {area = "", controller = "Home"});
            return redirect;
        }

        public ActionResult PersonalPage()
        {
            return View();
        }
    }
}