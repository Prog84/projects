using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Stalker.Entities
{
    public class StalkerIdentityUser : IdentityUser
    {
        public virtual UserInfo UserInfo { get; set; }

        [NotMapped]
        public string Password { get; set; }
    }
}