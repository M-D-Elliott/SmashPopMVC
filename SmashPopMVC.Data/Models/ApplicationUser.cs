using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Generic Profile data
        public DateTime MemberSince { get; set; }
        public bool IsActive { get; set; }

        // Social data
        public virtual ApplicationUser Partner { get; set; }
        public virtual IList<FriendRelationship> Friends { get; set; }

        // SmashPop specific data
        public virtual Character Main { get; set; }
        public virtual Character Alt { get; set; }
        public virtual Game FavoriteSmashGame { get; set; }
    }
}
