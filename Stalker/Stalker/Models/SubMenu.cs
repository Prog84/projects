using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stalker.Models
{
    public class SubMenu
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string CssIcon { get; set; }
        public int Order { get; set; }
    }
}