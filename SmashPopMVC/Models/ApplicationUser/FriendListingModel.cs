using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class FriendListingModel
    {
        public IEnumerable<UserListingModel> ApprovedFriends { get; set; }
        public IEnumerable<UserListingModel> RequestedFriends { get; set; }
        public IEnumerable<UserListingModel> FriendRequests { get; set; }
    }
}
