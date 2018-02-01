using System;
using System.Collections.Generic;

namespace Stalker.Entities
{
    public class Management
    {
        public int ManagementId { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? DateModified { get; set; }

        public List<Department> Departments { get; set; }

        public List<UserInfo> UserInfoes { get; set; }
    }
}