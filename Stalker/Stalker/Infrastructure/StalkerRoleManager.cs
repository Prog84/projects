using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Stalker.Entities;
using Stalker.Infrastructure.DbContexts;

namespace Stalker.Infrastructure
{
    public class StalkerRoleManager : RoleManager<StalkerIdentityRole>
    {
        public StalkerRoleManager(RoleStore<StalkerIdentityRole> store) : base(store)
        {
        }

        public static StalkerRoleManager Create(IdentityFactoryOptions<StalkerRoleManager> options, IOwinContext context)
        {
            return new StalkerRoleManager(new RoleStore<StalkerIdentityRole>(context.Get<StalkerDbContext>()));
        }
    }
}