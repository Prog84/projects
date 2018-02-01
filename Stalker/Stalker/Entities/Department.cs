using System;
using System.Collections.Generic;

namespace Stalker.Entities
{
    public class Department
    {
        public int? DepartmentId { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? DateModified { get; set; }

        public Management Management { get; set; }

        public int ManagementId { get; set; }

        public List<UserInfo> UserInfoes { get; set; }
    }
}