﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class UserFriendModel
    {
        public int RequestID { get; set; }
        public UserListingModel FriendData { get; set; }
    }
}