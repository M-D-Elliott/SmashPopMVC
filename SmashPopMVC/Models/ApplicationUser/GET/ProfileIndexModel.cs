using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Models.Friend;
using SmashPopMVC.Models.Vote;
using System.Collections.Generic;

namespace SmashPopMVC.Models.ApplicationUser
{
    public class ProfileIndexModel
    {
        public string ID { get; set; }
        public string CurrentUserID { get; set; }
        public bool CurrentUserFriend { get; set; }
        public bool IsCurrentUser { get; set; }
        public string Name { get; set; }
        public string Joined { get; set; }
        public CharacterDataModel Main { get; set; }
        public CharacterDataModel Alt { get; set; }
        public FriendListingModel Friends { get; set; }
        public UpdateViewModel UpdateViewModel { get; set; }
        public CommentListingModel Comments { get; set; }
        public VoteListingModel Votes { get; set; }

        public bool ThisUser { get; set; }
    }
}
