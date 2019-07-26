using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface IFriend
    {
        Task<Friend> Get(int? friendID, bool includeUsers = false);
        Task<Friend> GetByUsersID(string user1ID, string user2ID, bool includeUsers = false);
        Task<Friend> AddFriend(ApplicationUser user, ApplicationUser newFriend);
        Task<Friend> Update(int id, Friend friendship);
    }
}
