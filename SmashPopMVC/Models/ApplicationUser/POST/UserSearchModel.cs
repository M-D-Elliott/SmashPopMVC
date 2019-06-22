using SmashPopMVC.Models.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class UserSearchModel
    {
        public string SearchQuery { get; set; }
        public bool ByMain { get; set; }
        public bool ByAlt { get; set; }
        public bool ByScore { get; set; }
        public bool End { get; set; }
        public int Page { get; set; }
        public int Take { get; set; }
        public int Total { get; set; }
        public IEnumerable<UserListingModel> Results { get; set; }
    }
}
