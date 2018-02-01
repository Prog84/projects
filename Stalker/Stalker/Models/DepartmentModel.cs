using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Stalker.Entities;

namespace Stalker.Models
{
    public class DepartmentModel
    {
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "Это поле обязятельно для заполнения")]
        [DisplayName("Полное наименование*")]
        public string FullName { get; set; }

        [DisplayName("Сокращенное наименование")]
        public string ShortName { get; set; }

        public List<Management> Managements { get; set; }

        [Required(ErrorMessage = "Необходимо проставить управление")]
        public int SelectedManagementId { get; set; }
    }
}