
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Stalker.Models
{
    public class RegionModel
    {
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DisplayName("Название региона*")]
        public string RegionName { get; set; }
    }
}