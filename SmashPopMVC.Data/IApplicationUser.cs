using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
{
    public interface IApplicationUser
    {
        IEnumerable<ApplicationUser> SearchUsers(
            string searchQuery, 
            bool byMain = false, 
            bool byAlt = false, 
            bool byScore = false);

        ApplicationUser GetUser(
            string id, 
            bool friends = false);
        
    }
}
