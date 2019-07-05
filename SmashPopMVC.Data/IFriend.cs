﻿using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
{
    public interface IFriend
    {
        Friend Get(int friendID, bool includeUsers = false);
        void AddFriend(ApplicationUser user, ApplicationUser newFriend);
        bool AcceptFriend(int requestID);
    }
}
