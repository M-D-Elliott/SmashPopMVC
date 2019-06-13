using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class FriendListingModel
    {
        public IEnumerable<UserFriendModel> ApprovedFriends { get; set; }
        public IEnumerable<UserFriendModel> RequestedFriends { get; set; }
        public IEnumerable<UserFriendModel> FriendRequests { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
