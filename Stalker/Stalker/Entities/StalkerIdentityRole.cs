using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Stalker.Entities
{
    public class StalkerIdentityRole : IdentityRole
    {

        [DisplayName("Отображаемое имя")]
        [Required(ErrorMessage = "Заполните поле отображаемое имя")]
        public string DisplayName { get; set; }

        [DisplayName("Описание")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        public StalkerIdentityRole()
        {
            
        }

        public StalkerIdentityRole(string name) : base(name)
        {
            
        }
    }
}  