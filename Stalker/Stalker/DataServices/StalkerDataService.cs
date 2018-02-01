using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stalker.Entities;
using Stalker.Infrastructure.DbContexts;
using Stalker.Models;

namespace Stalker.DataServices
{
    public class StalkerDataService : IStalker
    {
        readonly StalkerDbContext _context = new StalkerDbContext();

        public StalkerDataService()
        {
            _context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));
        }

        public IEnumerable<Management> GetManagements()
        {
            return _context.Managements.Select(m => m);
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.Select(d => new
            {
                id = d.DepartmentId,
                fullName = d.FullName,
                shortName = d.ShortName,
                managementName = d.Management.FullName
            }).AsEnumerable()
                .Select(dp => new Department
                {
                    DepartmentId = dp.id,
                    FullName = dp.fullName,
                    ShortName = dp.shortName,
                    Management = new Management { FullName = dp.managementName}
                });
        }

        #region User
        public UserInfo GetUserInfo(string userId)
        {
            return _context.UserInfo.SingleOrDefault(ui => ui.StalkerIdentityUser.Id == userId);
        }

        public int UpdateUser(EditUserModel userEdit)
        {
            UserInfo userInfo = _context.UserInfo.SingleOrDefault(ui => ui.StalkerIdentityUserId == userEdit.Id);
            if (userInfo == null) return 0;

            userInfo.CityId = userEdit.SelectedCityId;
            userInfo.DateModified = DateTime.Now;
            userInfo.DepartmentId = userEdit.SelectedDepartmentId;
            userInfo.Family = userEdit.UserFamily;
            userInfo.Name = userEdit.UserName;
            userInfo.Otch = userEdit.UserOtch;
            userInfo.DateBirth = userEdit.UserDateBirth;
            userInfo.ManagementId = userEdit.SelectedManagementId;
            userInfo.RegionId = userEdit.SelectedRegionId;

            return _context.SaveChanges();
        }

        //Получение списка пользователей с основной информацией по каждому пользователю.
        public IEnumerable<UserBaseInfo> GetUsers()
        {
            IEnumerable<UserBaseInfo> users = _context.UserInfo
                .Select(ui => new
                {
                    id = ui.StalkerIdentityUserId,
                    login = ui.StalkerIdentityUser.UserName,
                    name = ui.Name,
                    family = ui.Family,
                    otch = ui.Otch,
                    dateBirth = ui.DateBirth,
                    cityName = ui.City.CityName,
                    managementName = ui.Management.FullName,
                    departmentName = ui.Department.FullName
                })
                .AsEnumerable()
                .Select(ud => new UserBaseInfo
                {
                    Id = ud.id.ToString(),
                    UserLogin = ud.login,
                    UserName = ud.name,
                    UserFamily = ud.family,
                    UserOtch = ud.otch,
                    UserDateBirth = ud.dateBirth,
                    City =  new CityModel {CityName = ud.cityName},
                    Management = new ManagementModel { FullName = ud.managementName},
                    Department = new DepartmentModel { FullName = ud.departmentName}
                });

            return users;
        }

        //Получение делатьной информации о пользователе
        public EditUserModel GetEditingUser(string id)
        {
            EditUserModel userDetails = _context.UserInfo
                .Select(ui => new
                {
                    id = ui.StalkerIdentityUserId,
                    login = ui.StalkerIdentityUser.UserName,
                    name = ui.Name,
                    family = ui.Family,
                    otch = ui.Otch,
                    dateBirth = ui.DateBirth,
                    city = ui.City,
                    region = ui.Region,
                    management = ui.Management,
                    department = ui.Department
                })
                .Where(ud => ud.id == id).AsEnumerable()
                .Select(ud => new EditUserModel
                {
                    Id = ud.id,
                    UserLogin = ud.login,
                    UserName = ud.name,
                    UserFamily = ud.family,
                    UserOtch = ud.otch,
                    UserDateBirth = ud.dateBirth,
                    SelectedRegionId = ud.region.RegionId,
                    SelectedCityId = ud.city.CityId,
                    SelectedManagementId = ud.management.ManagementId,
                    SelectedDepartmentId = ud.department?.DepartmentId
                }).SingleOrDefault();

            return userDetails;
        }
        #endregion

        #region Management
        //Создание управления
        public async Task<int> CrateManagementAsync(Management management)
        {
            _context.Managements.Add(management);
            int Id = await _context.SaveChangesAsync();
            return Id;
        }

        //Изменение данных управления
        public async Task<int> UpdateManagementAsync(Management newManagement)
        {
            var oldManagement = _context.Managements.SingleOrDefault(m => m.ManagementId == newManagement.ManagementId);
            if (oldManagement != null)
            {
                oldManagement.FullName = newManagement.FullName;
                oldManagement.ShortName = newManagement.ShortName;
                oldManagement.DateModified = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }
        #endregion

        #region Department
        //Изменение отдела
        public async Task<int> UpdateDepartmentAsync(Department newDepartment)
        {
            var oldDepartment = _context.Departments.SingleOrDefault(m => m.DepartmentId == newDepartment.DepartmentId);
            if (oldDepartment != null)
            {
                oldDepartment.FullName = newDepartment.FullName;
                oldDepartment.ShortName = newDepartment.ShortName;
                oldDepartment.ManagementId = newDepartment.ManagementId;
                oldDepartment.DateModified = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }

        //Создание отдела
        public async Task<int> CreateDepartmentAsync(Department department)
        {
            _context.Departments.Add(department);
            return await _context.SaveChangesAsync();
        }

        //Получить отдел по его Id
        public Department GetDepartmentById(int? departmentId)
        {
            return _context.Departments.SingleOrDefault(d => d.DepartmentId == departmentId);
        }

        //Получить управление по его Id
        public Management GetManagementById(int? managementId)
        {
            return _context.Managements.SingleOrDefault(m => m.ManagementId == managementId);
        }

        public IEnumerable<DepartmentModel> GetDepartmentsByManagement(int managementId)
        {
            return _context.Departments.Select(d => new
            {
                id = d.DepartmentId,
                name = d.FullName,
                managementId = d.ManagementId
            }).AsEnumerable().Where(s => s.managementId == managementId)
                .Select(dp => new DepartmentModel
                {
                    DepartmentId = dp.id,
                    FullName = dp.name
                });
        }
        #endregion

        #region Region
        //Получение списка регионов
        public IEnumerable<RegionModel> GetRegions()
        {
            return _context.Regions.Select(r => new
            {
                regionId = r.RegionId,
                regionName = r.RegionName
            }).AsEnumerable()
                .Select(region => new RegionModel
                {
                    RegionId = region.regionId,
                    RegionName = region.regionName
                });
        }

        //Создать регион
        public RegionModel CreateRegion(RegionModel regionModel)
        {
            Region region = new Region
            {
                RegionName = regionModel.RegionName
            };
            _context.Regions.Add(region);
            _context.SaveChanges();
            return new RegionModel
            {
                RegionId = region.RegionId,
                RegionName = region.RegionName
            };
        }

        //Получить регион по Id
        public RegionModel GetRegion(int regionId)
        {
            return _context.Regions.Where(r => r.RegionId == regionId)
                .Select(region => new RegionModel
                {
                    RegionName = region.RegionName
                }).SingleOrDefault();
        }

        //Обновить регион
        public RegionModel UpdateRegion(RegionModel regionModel)
        {
            Region region = _context.Regions.SingleOrDefault(r => r.RegionId == regionModel.RegionId);
            if (region == null)
                return null;
            region.RegionName = regionModel.RegionName;
            _context.SaveChanges();

            return new RegionModel {RegionId = region.RegionId, RegionName = region.RegionName};
        }
        #endregion

        #region City
        //Получить список городов
        public IEnumerable<CityModel> GetCityes()
        {
            return _context.Cities.Select(city => new
            {
                cityId = city.CityId,
                cityName = city.CityName,
                regionName = city.Region.RegionName,
                regionId = city.Region.RegionId
            }).AsEnumerable().Select(c => new CityModel
            {
                CityId = c.cityId,
                CityName = c.cityName,
                RegionName = c.regionName,
                RegionId = c.regionId
            });
        }

        public IEnumerable<CityModel> GetCitiesByRegion(int regionId)
        {
            return _context.Cities.Select(city => new
            {
                cityId = city.CityId,
                cityName = city.CityName,
                regionName = city.Region.RegionName,
                regionId = city.Region.RegionId
            })
            .Where(c => c.regionId == regionId)
            .AsEnumerable().Select(c => new CityModel
            {
                CityId = c.cityId,
                CityName = c.cityName,
                RegionName = c.regionName,
                RegionId = c.regionId
            });
        }

        //Создать населенный пункт
        public async Task<int?> CreateCityAsync(CityModel cityModel)
        {
            Region region = _context.Regions.SingleOrDefault(r => r.RegionId == cityModel.RegionId);
            if (region == null) return null;
            City city = new City
            {
                CityName = cityModel.CityName,
                Region = region
            };
            _context.Cities.Add(city);
            return await _context.SaveChangesAsync();
        }

        // Получить населенный пункт по Id
        public CityModel GetCity(int? cityId)
        {
            return _context.Cities.Select(city => new
            {
                id = city.CityId,
                name = city.CityName,
                regionId = city.Region.RegionId
            }).Where(c => c.id == cityId).AsEnumerable().Select(s => new CityModel
            {
                CityId = s.id,
                CityName = s.name,
                Regions = GetRegions().ToList(),
                RegionId = s.regionId
            }).SingleOrDefault();
        }

        //Изменение населенного пункта
        public int UpdateCity(CityModel cityModel)
        {
            City city = _context.Cities.SingleOrDefault(c => c.CityId == cityModel.CityId);
            if (city == null) return 0;
            city.CityName = cityModel.CityName;
            city.Region = _context.Regions.SingleOrDefault(r => r.RegionId == cityModel.RegionId);
            return _context.SaveChanges();
        }
        #endregion
    }
}