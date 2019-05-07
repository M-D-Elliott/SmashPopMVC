using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.UserProfile
{
    public class ProfileIndexModel
    {
        public string Name { get; set; }
        public string MainName { get; set; }
        public string MainImage { get; set; }
        public string AltName { get; set; }
        public string AltImage { get; set; }
        public string PartnerName { get; set; }
        public string PartnerMainImage { get; set; }
        public IEnumerable<FriendListingModel> Friends { get; set; } 
    }
}
