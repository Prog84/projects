using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Stalker.DataServices;
using Stalker.Entities;
using Stalker.Infrastructure;
using Stalker.Models;
using System.Net;

namespace Stalker.Areas.Administration.Controllers
{
    [MenuItem(Title = "Администрирование", IsClickable = false)]
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IStalker _stalker;
        private StalkerUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<StalkerUserManager>();

        public AdminController(IStalker stalker)
        {
            _stalker = stalker;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region User
        //Просмотр списка пользователей
        [MenuItem(Title = "Пользователи", CssIcon = "fa fa-users fa-lg fa-fw")]
        public ActionResult Users()
        {
            return View(_stalker.GetUsers().ToList());
        }

        //Создание пользователя
        public ActionResult CreateUser()
        {
            List<Department> departments = _stalker.GetDepartments().ToList();
            departments.Add(new Department { FullName = "Не установлено" });
            ViewBag.SelectedRegionId = new SelectList(_stalker.GetRegions(), "RegionId", "RegionName");
            ViewBag.SelectedCityId = new SelectList(new List<CityModel>(), "CityId", "CityName");
            ViewBag.SelectedManagementId = new SelectList(_stalker.GetManagements(), "ManagementId", "FullName");
            ViewBag.SelectedDepartmentId = new SelectList(departments, "DepartmentId", "FullName");

            return View(new UserDetailsModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserDetailsModel model)
        {
            if (ModelState.IsValid)
            {
               /* List<UserPhone> phones = new List<UserPhone>();
                if (model.PhoneNumber != null)
                {
                    phones.Add(new UserPhone { Phone1 = model.PhoneNumber, DateCreatePhone1 = DateTime.Now });
                }*/
                
                UserInfo newUserInfo = new UserInfo
                {
                    Family = model.UserFamily,
                    Name = model.UserName,
                    Otch = model.UserOtch,
                    DateBirth = model.UserDateBirth,
                    CityId = model.SelectedCityId,
                    RegionId = model.SelectedRegionId,
                    ManagementId = model.SelectedManagementId,
                    DepartmentId = model.SelectedDepartmentId == 0 ? null : model.SelectedDepartmentId,
                    DateCreate = DateTime.Now,
                    //UserPhones = phones.Any() ? phones : null
                };

                StalkerIdentityUser newUser = new StalkerIdentityUser
                {
                    UserName = model.UserLogin,
                    UserInfo = newUserInfo
                };

                IdentityResult createUserResult = await UserManager.CreateAsync(newUser, model.Password);
                if (!createUserResult.Succeeded)
                {
                    TempData[Message.ErrorMessage] = "Не удалось создать пользователя";
                    return RedirectToAction("Users");
                }

                TempData[Message.SuccessMessage] = $"Пользователь \"{model.UserLogin}\" успешно создан";
                return RedirectToAction("Users");
            }

            List<Department> departments = _stalker.GetDepartments().ToList();
            departments.Add(new Department { FullName = "Не установлено" });
            ViewBag.SelectedRegionId = new SelectList(_stalker.GetRegions(), "RegionId", "RegionName");
            ViewBag.SelectedCityId = new SelectList(new List<CityModel>(), "CityId", "CityName");
            ViewBag.SelectedManagementId = new SelectList(_stalker.GetManagements(), "ManagementId", "FullName");
            ViewBag.SelectedDepartmentId = new SelectList(departments, "DepartmentId", "FullName");

            return View(new UserDetailsModel());
        }

        //Изменение учетных данных пользователя
        public ActionResult EditUser(string id)
        {
            
            EditUserModel userDetails = _stalker.GetEditingUser(id);
            if (userDetails == null)
            {
                TempData[Message.ErrorMessage] = $"Информация о пользователе с id = \"{id}\" не найдена";
                return RedirectToAction("Users");
            }

            List<DepartmentModel> departments = _stalker.GetDepartmentsByManagement(userDetails.SelectedManagementId).ToList();
            departments.Add(new DepartmentModel { DepartmentId = null, FullName = "Не установлено" });

            ViewBag.SelectedRegionId = new SelectList(_stalker.GetRegions(),
                "RegionId", "RegionName", userDetails.SelectedRegionId);
            ViewBag.SelectedCityId = new SelectList(_stalker.GetCitiesByRegion(userDetails.SelectedRegionId),
                "CityId", "CityName", userDetails.SelectedCityId);
            ViewBag.SelectedManagementId = new SelectList(_stalker.GetManagements(),
                "ManagementId", "FullName", userDetails.SelectedManagementId);
            ViewBag.SelectedDepartmentId = new SelectList(departments,
                "DepartmentId", "FullName", userDetails.SelectedDepartmentId);

            userDetails.LockoutEnabled = UserManager.GetLockoutEnabled(userDetails.Id);
            return View(userDetails);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUserModel userDetails)
        {
            if (ModelState.IsValid)
            {
                int countSaved = _stalker.UpdateUser(userDetails);
                if (countSaved != 1)
                {
                    TempData[Message.ErrorMessage] = "Изменения выполнить не удалось";
                    return RedirectToAction("Users");
                }
                TempData[Message.SuccessMessage] =
                    $"Изменения учетной записи пользователя \"{userDetails.UserLogin}\" сохранены";
                return RedirectToAction("Users");
            }
            EditUserModel model = _stalker.GetEditingUser(userDetails.Id);
            return View(model);
        }

        //Блокировка пользователя
        public async Task<ActionResult> Block(string userId)
        {
            IdentityResult result = await UserManager.SetLockoutEnabledAsync(userId, true);
            if (!result.Succeeded)
            {
                TempData[Message.ErrorMessage] = "Блокировка пользователя не выполнена";
                return RedirectToAction("EditUser", new { id = userId });
            }
            TempData[Message.SuccessMessage] = $"Пользователь \"{UserManager.FindById(userId).UserName}\" заблокирован";
            return RedirectToAction("EditUser", new { id = userId });
        }

        //Разблокировка пользователя
        public async Task<ActionResult> Unblock(string userId)
        {
            IdentityResult result = await UserManager.SetLockoutEnabledAsync(userId, false);
            if (!result.Succeeded)
            {
                TempData[Message.ErrorMessage] = "Не удалось разблокировать пользовтеля";
                return RedirectToAction("EditUser", new { id = userId });
            }
            TempData[Message.SuccessMessage] = $"Пользователь \"{UserManager.FindById(userId).UserName}\" разблокирован";
            return RedirectToAction("EditUser", new { id = userId });
        }

        // сброс пароля пользователя
        // GET: /AccountAdmin/ResetPassword
        public ActionResult ResetPassword(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResetPasswordModel model = new ResetPasswordModel() { Id = userId };
            return View(model);
        }

        //
        // POST: /AccountAdmin/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var removePassword = UserManager.RemovePassword(model.Id);
            if (removePassword.Succeeded)
            {
                //Removed Password Success
                var AddPassword = UserManager.AddPassword(model.Id, model.NewPassword);
                if (AddPassword.Succeeded)
                {
                    //return View("PasswordResetConfirm");
                    TempData[Message.SuccessMessage] = $"Пароль пользователя \"{UserManager.FindById(model.Id).UserName}\" изменен";
                    return RedirectToAction("EditUser", new { id = model.Id });
                }
                else
                {
                    TempData[Message.ErrorMessage] = "Не удалось изменить пароль";
                    return RedirectToAction("EditUser", new { id = model.Id });
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion


        #region Management
        //Просмотр списка управлений
        [MenuItem(Title = "Управления")]
        public ActionResult Managements()
        {
            var managements = _stalker.GetManagements();
            List<ManagementModel> managementModels = new List<ManagementModel>();
            managementModels.AddRange(
                managements.Select(m => new ManagementModel {ManagementId = m.ManagementId, FullName = m.FullName, ShortName = m.ShortName}));

            return View(managementModels);
        }

        //Создание управления
        public ActionResult CreateManagement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateManagement(ManagementModel managementModel)
        {
            if (ModelState.IsValid)
            {
                await _stalker.CrateManagementAsync(new Management
                {
                    FullName = managementModel.FullName,
                    ShortName = managementModel.ShortName,
                    DateCreate = DateTime.Now
                });
            }
            else
            {
                return View();
            }

            TempData["successMessage"] = 
                $"Управление \"{managementModel.FullName}\" успешно сохранено.";

            return RedirectToAction("Managements");
        }

        //Редактирование управления
        public ActionResult EditManagement(int? mangementId)
        {
            if (mangementId == null)
            {
                List<string> errorMessages = new List<string> {"Ошибка передачи параметра managementId в метод действия."};
                return View("Error", errorMessages);
            }

            Management management = _stalker.GetManagementById(mangementId);
            if (management == null)
            {
                List<string> errorMessages = new List<string> {$"Не удалось получить управление с Id = {mangementId}"};
                return View("Error", errorMessages);
            }

            return View(new ManagementModel { ManagementId = management.ManagementId, FullName = management.FullName, ShortName = management.ShortName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditManagement(ManagementModel managementModel)
        {
            if (ModelState.IsValid)
            {
                int countUpdates = await _stalker.UpdateManagementAsync(new Management
                {
                    ManagementId = managementModel.ManagementId,
                    FullName = managementModel.FullName,
                    ShortName = managementModel.ShortName
                });

                if (countUpdates == 1)
                {
                    TempData["successMessage"] =
                        $"Изменения в управлении \"{managementModel.FullName}\" успешно сохранены.";
                }
                else
                {
                    TempData["errorMessage"] = "Не удалось сохранить изменения.";
                }

                return RedirectToAction("Managements");
            }
            return View();
        }
        #endregion

        #region Department
        //Просмотр списка отделов
        [MenuItem(Title = "Отделы")]
        public ActionResult Departments()
        {
            List<Department> departmnents = new List<Department>();
            departmnents.AddRange(_stalker.GetDepartments());
            return View(departmnents);
        }

        //Создание отдела
        public ActionResult CreateDepartment()
        {
            DepartmentModel departmentModel = new DepartmentModel();
            List<Management> managements = new List<Management>();
            managements.AddRange(_stalker.GetManagements());
            departmentModel.Managements = managements;

            return View(departmentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDepartment(DepartmentModel department)
        {
            if (ModelState.IsValid)
            {
                await _stalker.CreateDepartmentAsync(new Department
                {
                    FullName = department.FullName,
                    ShortName = department.ShortName,
                    DateCreate = DateTime.Now,
                    ManagementId = department.SelectedManagementId
                });
            }
            else
            {
                DepartmentModel departmentModel = new DepartmentModel();
                List<Management> managements = new List<Management>();
                managements.AddRange(_stalker.GetManagements());
                departmentModel.Managements = managements;

                return View(departmentModel);
            }

            TempData["successMessage"] = $"Отдел \"{department.FullName}\" успешно создан.";
            return RedirectToAction("Departments");
        }

        //Изменение существующего отдела
        public ActionResult EditDepartment(int? departmentId)
        {
            if (departmentId == null)
            {
                List<string> errorMessages = new List<string> {"Ошибка передачи параметра id для данного отдела"};
                return View("Error", errorMessages);
            }

            Department department = _stalker.GetDepartmentById(departmentId);
            if (department == null)
            {
                List<string> errorMessages = new List<string> { $"Не удалось извлечь из БД отдел с Id = {departmentId}" };
                return View("Error", errorMessages);
            }

            return
                View(new DepartmentModel
                {
                    FullName = department.FullName,
                    ShortName = department.ShortName,
                    SelectedManagementId = department.ManagementId,
                    DepartmentId = department.DepartmentId,
                    Managements = _stalker.GetManagements().ToList()
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDepartment(DepartmentModel departmentModel)
        {
            if (ModelState.IsValid)
            {
                int countUpdates = await _stalker.UpdateDepartmentAsync(new Department
                {
                    DepartmentId = departmentModel.DepartmentId,
                    FullName = departmentModel.FullName,
                    ShortName = departmentModel.ShortName,
                    ManagementId = departmentModel.SelectedManagementId
                });

                if (countUpdates == 1)
                {
                    TempData[Message.SuccessMessage] =
                        $"Изменения в отделе \"{departmentModel.FullName}\" успешно сохранены.";
                }
                else
                {
                    TempData[Message.ErrorMessage] = "Не удалось сохранить изменения.";
                }

                return RedirectToAction("Departments");
            }

            return
                View(new DepartmentModel
                {
                    FullName = departmentModel.FullName,
                    ShortName = departmentModel.ShortName,
                    SelectedManagementId = departmentModel.SelectedManagementId,
                    DepartmentId = departmentModel.DepartmentId,
                    Managements = _stalker.GetManagements().ToList()
                });
        }
        #endregion

        #region Region
        //Список регионов
        [MenuItem(Title = "Регионы")]
        public ActionResult Regions()
        {
            List<RegionModel> regions = _stalker.GetRegions().ToList();
            return View("Regions", regions);
        }

        //Создание региона
        public ActionResult CreateRegion()
        {
            return View("CreateRegion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRegion(RegionModel region)
        {
            if (ModelState.IsValid)
            {
                _stalker.CreateRegion(region);
                TempData[Message.SuccessMessage] = $"Регион \"{region.RegionName}\" успешно сохранен";
                return RedirectToAction("Regions");
            }
            return View("CreateRegion", region);
        }

        //Правка названия региона
        public ActionResult EditRegion(int regionId)
        {
            RegionModel regionModel = _stalker.GetRegion(regionId);
            if (regionModel == null)
            {
                TempData[Message.ErrorMessage] = $"Регион с Id = {regionId} не найден";
                return RedirectToAction("Regions");
            }
            return View("EditRegion", regionModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRegion(RegionModel region)
        {
            if (ModelState.IsValid)
            {
                var savedRegionModel = _stalker.UpdateRegion(region);
                if (savedRegionModel == null) //регион не найден
                    TempData[Message.ErrorMessage] = $"Не удалось внести изменения в регионе {region.RegionName}";
                else
                    TempData[Message.SuccessMessage] = $"Изменения в регионе {region.RegionName} успешно сохранены";

                return RedirectToAction("Regions");
            }
            return View("EditRegion", region);
        }
        #endregion

        #region City
        //Города
        [MenuItem(Title = "Города")]
        public ActionResult Cities()
        {
            return View(_stalker.GetCityes().ToList());
        }

        //Правка города
        public ActionResult EditCity(int cityid)
        {
            return View(_stalker.GetCity(cityid));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCity(CityModel cityModel)
        {
            if (ModelState.IsValid)
            {
                int id = _stalker.UpdateCity(cityModel);
                if (id == 0)
                {
                    TempData[Message.ErrorMessage] = "Изменения внесены не были";
                    return RedirectToAction("Cities");
                }
                TempData[Message.SuccessMessage] = $"Обновление населенного пункта {cityModel.CityName} выполнено";
                return RedirectToAction("Cities");
            }
            return View(_stalker.GetCity(cityModel.CityId));
        }

        //Создание населенного пункта
        public ActionResult CreateCity()
        {
            CityModel city = new CityModel() {Regions = _stalker.GetRegions().ToList()};
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCity(CityModel cityModel)
        {
            if (ModelState.IsValid)
            {
                int? i = await _stalker.CreateCityAsync(cityModel);
                if (i == null)
                {
                    TempData[Message.ErrorMessage] = "Не удалось создать населенный пункт";
                    return RedirectToAction("Cities");
                }

                TempData[Message.SuccessMessage] = $"Населенный пункт \"{cityModel.CityName}\" успешно создан";
                return RedirectToAction("Cities");
            }

            CityModel city = new CityModel() { Regions = _stalker.GetRegions().ToList() };
            return View(city);
        }

        public JsonResult GetCitiesByRegion(int selectedRegionId)
        {
            IEnumerable<CityModel> cities = _stalker.GetCitiesByRegion(selectedRegionId);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentsByManagement(int managementId)
        {
            IEnumerable<DepartmentModel> departments = _stalker.GetDepartmentsByManagement(managementId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}