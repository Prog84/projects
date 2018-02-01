using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Stalker.Models
{
    public class ManagementModel
    {
        public int ManagementId { get; set; }

        [Required(ErrorMessage = "Это поле обязятельно для заполнения", AllowEmptyStrings = false)]
        [DisplayName("Полное наименование*")]
        public string FullName { get; set; }

        [DisplayName("Сокращенное наименование")]
        public string ShortName { get; set; }
    }
}