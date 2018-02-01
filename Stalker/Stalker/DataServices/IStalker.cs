using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stalker.Entities;
using Stalker.Models;

namespace Stalker.DataServices
{
    public interface IStalker
    {
        IEnumerable<Management> GetManagements();
        Task<int> CrateManagementAsync(Management management);
        Task<int> UpdateManagementAsync(Management management);
        Management GetManagementById(int? managementId);

        IEnumerable<Department> GetDepartments();
        Task<int> CreateDepartmentAsync(Department department);
        Task<int> UpdateDepartmentAsync(Department department);
        Department GetDepartmentById(int? departmentId);
        IEnumerable<DepartmentModel> GetDepartmentsByManagement(int managementId);

        IEnumerable<UserBaseInfo> GetUsers();
        UserInfo GetUserInfo(string userId);
        int UpdateUser(EditUserModel userDetails);
        EditUserModel GetEditingUser(string id);

        IEnumerable<RegionModel> GetRegions();
        RegionModel CreateRegion(RegionModel regionModel);
        RegionModel GetRegion(int regionId);
        RegionModel UpdateRegion(RegionModel regionModel);

        IEnumerable<CityModel> GetCityes();
        IEnumerable<CityModel> GetCitiesByRegion(int regionId);
        Task<int?> CreateCityAsync(CityModel cityModel);
        CityModel GetCity(int? cityId);
        int UpdateCity(CityModel cityModel);
    }
}
