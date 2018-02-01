using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Stalker.Entities;

namespace Stalker.Models
{
    public class CityModel
    {
        public int CityId { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DisplayName("Название населенного пункта")]
        public string CityName { get; set; }

        public string RegionName { get; set; }

        public List<RegionModel> Regions { get; set; }

        [DisplayName("Регион")]
        [Required(ErrorMessage = "Необходимо выбрать регион")]
        public int RegionId { get; set; }
    }
}