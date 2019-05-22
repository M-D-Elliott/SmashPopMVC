using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            SentFriendRequests = new List<Friend>();
            ReceievedFriendRequests = new List<Friend>();
        }

        // Generic Profile data
        public string ShortName { get; set; }
        public DateTime MemberSince { get; set; }
        public bool IsActive { get; set; }

        // Social data
        public virtual ApplicationUser Partner { get; set; }
        public virtual ICollection<Friend> SentFriendRequests { get; set; }
        public virtual ICollection<Friend> ReceievedFriendRequests { get; set; }

        // SmashPop specific data
        public virtual Character Main { get; set; }
        public virtual Character Alt { get; set; }
        public virtual Game FavoriteSmashGame { get; set; }

        [NotMapped]
        public virtual ICollection<ApplicationUser> FriendsApproved
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.Approved).Select(f => f.RequestedTo).ToList();
                friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved).Select(f => f.RequestedBy));
                return friends;
            }
        }

        [NotMapped]
        public virtual ICollection<ApplicationUser> Friends
        {
            get
            {
                var friends = SentFriendRequests.Select(f => f.RequestedTo).ToList();
                friends.AddRange(ReceievedFriendRequests.Select(f => f.RequestedBy));
                return friends;
            }
        }

        [NotMapped]
        public virtual ICollection<Friend> FriendRequestsSent
        {
            get
            {
                return SentFriendRequests.Where(x => !x.Approved).ToList();
            }
        }

        [NotMapped]
        public virtual ICollection<Friend> FriendRequestsReceived
        {
            get
            {
                return ReceievedFriendRequests.Where(x => !x.Approved).ToList();
            }
        }
    }
}
