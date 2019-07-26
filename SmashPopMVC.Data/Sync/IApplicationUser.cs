using SmashPopMVC.Data.Models;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Sync
{
    public interface IApplicationUser
    {
        List<ApplicationUser> SearchUsers(
            string searchQuery, 
            bool byMain = false, 
            bool byAlt = false, 
            bool byScore = false
            );

        ApplicationUser Get(string id, bool social = false);
        ApplicationUser Update(string id, ApplicationUser user);
        bool ShortNameExists(string shortName);
    }
}
