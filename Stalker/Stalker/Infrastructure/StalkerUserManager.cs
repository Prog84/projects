using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Stalker.Entities;
using Stalker.Infrastructure.DbContexts;

namespace Stalker.Infrastructure
{
    public class StalkerUserManager : UserManager<StalkerIdentityUser>
    {
        public StalkerUserManager(IUserStore<StalkerIdentityUser> store) : base(store)
        {

        }

        public static StalkerUserManager Create(IdentityFactoryOptions<StalkerUserManager> options, IOwinContext context)
        {
            StalkerDbContext db = context.Get<StalkerDbContext>();
            StalkerUserManager manager = new StalkerUserManager(new UserStore<StalkerIdentityUser>(db))
            {
                PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6
                }
            };


            manager.UserValidator = new UserValidator<StalkerIdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true
            };

            return manager;
        }
    }
}