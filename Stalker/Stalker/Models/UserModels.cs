using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Stalker.Entities;

namespace Stalker.Models
{
    public class UserBaseInfo
    {
        public string Id { get; set; }

        [DisplayName("Имя")]
        public string UserName { get; set; }

        [DisplayName("Фамилия")]
        public string UserFamily { get; set; }

        [DisplayName("Отчество")]
        public string UserOtch { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Введите дату в формате ДД.ММ.ГГГГ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM.dd.yyyy}")]
        [DisplayName("Дата рождения")]
        public DateTime? UserDateBirth { get; set; }

        [DisplayName("Регион")]
        public RegionModel Region { get; set; }

        [DisplayName("Город")]
        public CityModel City { get; set; }

        [DisplayName("Номер телефона (сотовый)")]
        [StringLength(maximumLength: 12, MinimumLength = 12, ErrorMessage = "Формат номера: +7 123 456 78 90(без пробелов)")]
        public string PhoneNumber { get; set; }

        [DisplayName("Логин*")]
        [Required(ErrorMessage = "Поле логин обязательно для заполнения")]
        public string UserLogin { get; set; }

        [DisplayName("Орган")]
        public ManagementModel Management { get; set; }

        [DisplayName("Подразделение")]
        public DepartmentModel Department { get; set; }
    }

    public class LoginModel
    {
        [Required (ErrorMessage = "Поле логин не должно быть пустым")]
        [DisplayName ("Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле пароль не должно быть пустым")]
        [DisplayName("Пароль")]
        public string Password { get; set; }
    }

    public class RoleEditModel
    {
        public StalkerIdentityRole Role { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<StalkerIdentityUser> Members { get; set; }
        public IEnumerable<StalkerIdentityUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        public string RoleName { get; set; }
        [Required(ErrorMessage = "Необходимо заполнить поле имени группы")]
        public string DisplayName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }

    public class EditUserModel : UserBaseInfo
    {
        [DisplayName("Управление")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int SelectedManagementId { get; set; }

        [DisplayName("Отдел")]
        public int? SelectedDepartmentId { get; set; }

        [DisplayName("Регион")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int SelectedRegionId { get; set; }

        [DisplayName("Населенный пункт")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int SelectedCityId { get; set; }

        public bool LockoutEnabled { get; set; }
    }

    public class UserDetailsModel : EditUserModel
    {
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword", ErrorMessage = "Пароли не совпадают")]
        [Required(ErrorMessage = "Поле пароль обязятельно для заполнения")]
        [DisplayName("Пароль*")]
        public string Password { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Required(ErrorMessage = "Поле пароль обязятельно для заполнения")]
        [DisplayName("Повторите пароль*")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordModel 
    {
        public string Id { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword", ErrorMessage = "Пароли не совпадают")]
        [Required(ErrorMessage = "Поле пароль обязятельно для заполнения")]
        [DisplayName("Пароль*")]
        public string NewPassword { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Required(ErrorMessage = "Поле пароль обязятельно для заполнения")]
        [DisplayName("Повторите пароль*")]
        public string ConfirmPassword { get; set; }
    }
}