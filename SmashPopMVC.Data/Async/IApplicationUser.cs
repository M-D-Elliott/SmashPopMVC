using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface IApplicationUser
    {
        Task<List<ApplicationUser>> SearchUsers(
            string searchQuery, 
            bool byMain = false, 
            bool byAlt = false, 
            bool byScore = false
            );

        Task<ApplicationUser> Get(string id, bool social = false);
        Task<ApplicationUser> Update(string id, ApplicationUser user);
        Task<bool> ShortNameExists(string shortName);
    }
}
