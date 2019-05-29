using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            bool social = false);
        void UpdateUserCharacters(string userID, Character new_main, Character new_alt);
    }
}
