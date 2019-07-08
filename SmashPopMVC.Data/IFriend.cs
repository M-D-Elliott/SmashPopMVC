using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
{
    public interface IFriend
    {
        Friend Get(int friendID, bool includeUsers = false);
        Friend GetByUsersID(string user1ID, string user2ID, bool includeUsers = false);
        void AddFriend(ApplicationUser user, ApplicationUser newFriend);
        void Update(Friend friendship);
    }
}
