using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stalker.Entities
{
    public class City
    {
        public int CityId { get; set; }

        public string CityName { get; set; }

        public Region Region { get; set; }

        public List<UserInfo> UserInfoes { get; set; }
    }
}