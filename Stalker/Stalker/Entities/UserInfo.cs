using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stalker.Entities
{
    public class UserInfo
    {
        public string StalkerIdentityUserId { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string Otch { get; set; }

        public DateTime? DateBirth { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? DateModified { get; set; }

        public StalkerIdentityUser StalkerIdentityUser { get; set; }

        public Department Department { get; set; }

        public int? DepartmentId { get; set; }

        public Management Management { get; set; }
        public int ManagementId { get; set; }

        public City City { get; set; }
        public int CityId { get; set; }

        public Region Region { get; set; }
        public int RegionId { get; set; }
    }
}