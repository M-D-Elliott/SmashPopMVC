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
            Comments = new List<Comment>();
            Votes = new List<Vote>();
        }

        // Generic Profile data
        public string ShortName { get; set; }
        public DateTime MemberSince { get; set; }
        public bool IsActive { get; set; }

        // Social data
        public virtual ApplicationUser Partner { get; set; }
        public virtual ICollection<Friend> SentFriendRequests { get; set; }
        public virtual ICollection<Friend> ReceievedFriendRequests { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }
        public virtual IEnumerable<Vote> Votes { get; set; }

        // SmashPop specific data
        public virtual Character Main { get; set; }
        public virtual Character Alt { get; set; }
        public virtual Game FavoriteSmashGame { get; set; }

    }
}
