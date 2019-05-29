using SmashPopMVC.Models.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class UserSearchModel
    {
        public IEnumerable<UserListingModel> Results { get; set; }

        public string SearchQuery { get; set; }
        public bool ByMain { get; set; }
        public bool ByAlt { get; set; }
        public bool ByScore { get; set; }
    }
}
