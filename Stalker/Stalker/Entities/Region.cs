using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stalker.Entities
{
    public class Region
    {
        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public List<City> Cities { get; set; }

        public List<UserInfo> UserInfoes { get; set; }
    }
}