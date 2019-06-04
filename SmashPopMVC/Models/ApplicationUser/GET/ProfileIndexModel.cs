using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Models.Vote;
using System.Collections.Generic;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class ProfileIndexModel
    {
        public string ID { get; set; }
        public string CurrentUserID { get; set; }
        public bool CurrentUserIsFriends { get; set; }
        public string Name { get; set; }
        public CharacterDataModel Main { get; set; }
        public CharacterDataModel Alt { get; set; }
        public string PartnerName { get; set; }
        public string PartnerMainImage { get; set; }
        public FriendListingModel Friends { get; set; }
        public UpdateViewModel UpdateViewModel { get; set; }
        public IEnumerable<CommentDataModel> Comments { get; set; }
        public VoteListingModel Votes { get; set; }

        public bool ThisUser { get; set; }
    }
}
