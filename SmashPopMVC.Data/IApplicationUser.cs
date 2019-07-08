using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Data
{
    public interface IApplicationUser
    {
        IList<ApplicationUser> SearchUsers(
            string searchQuery, 
            bool byMain = false, 
            bool byAlt = false, 
            bool byScore = false
            );

        ApplicationUser Get(
            string id, 
            bool social = false);
        void Update(ApplicationUser user);
    }
}
