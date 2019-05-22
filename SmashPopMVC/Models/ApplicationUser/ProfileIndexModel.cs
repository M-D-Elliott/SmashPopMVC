using System.Collections.Generic;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class ProfileIndexModel
    {
        public string ID { get; set; }
        public string CurrentUserID { get; set; }
        public bool CurrentUserIsFriends { get; set; }
        public string Name { get; set; }
        public string MainName { get; set; }
        public string MainImage { get; set; }
        public string AltName { get; set; }
        public string AltImage { get; set; }
        public string PartnerName { get; set; }
        public string PartnerMainImage { get; set; }
        public FriendListingModel Friends { get; set; }
        public UpdateViewModel UpdateViewModel { get; set; }

        public bool ThisUser { get; set; }
    }
}
