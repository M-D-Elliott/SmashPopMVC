using SmashPopMVC.Models.Friend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class FriendListingModel
    {
        public ICollection<UserFriendModel> ApprovedFriends { get; set; }
        public ICollection<UserFriendModel> RequestedFriends { get; set; }
        public ICollection<UserFriendModel> FriendRequests { get; set; }
        public UserFriendModel CurrentUserFriend { get; set; }
        public UserFriendModel PartnerFriend { get; set; }
        public bool IsCurrentUser { get; set; }
        public RequestPartnershipViewModel RequestPartnershipViewModel { get; set; }
    }
}
