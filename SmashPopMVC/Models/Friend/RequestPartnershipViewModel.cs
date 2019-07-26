using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Friend
{
    public class RequestPartnershipViewModel
    {
        public string CurrentUserID { get; set; }
        public int? FriendID { get; set; }
    }
}
