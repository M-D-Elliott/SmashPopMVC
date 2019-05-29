using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SmashPopMVC.Data.Models;
using System.Linq;
using SmashPopMVC.Data;
using Microsoft.AspNetCore.Authorization;
using SmashPopMVC.Service.Validators;
using System.Collections.Generic;
using SmashPopMVC.Models.ApplicationUser;
using System;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Models.Vote;
using SmashPopMVC.Models.Character;

namespace SmashPopMVC.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _applicationUserService;
        private readonly ICharacter _characterService;
        private readonly IFriend _friendService;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, 
                                         IApplicationUser applicationUserService, 
                                         ICharacter characterService, 
                                         IFriend friendService)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _characterService = characterService;
            _friendService = friendService;
        }

        [AllowAnonymous]
        public IActionResult Profile(string id = null)
        { 

            if(id == null)
            {
                if(User.Identity.IsAuthenticated)
                {
                    id = _userManager.GetUserId(User);
                }
                else
                {
                    return NotFound("You are not signed in.");
                }
            }

            var user = _applicationUserService.GetUser(id, social: true);

            if(user != null)
            {
                var model = BuildUserProfile(user);

                return View(model);
            }
            return NotFound("User not found.");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Search(string query, bool byMain, bool byAlt, bool byScore)
        {
            return RedirectToAction("Results", new { query, byMain, byAlt, byScore });
        }

        [AllowAnonymous]
        public IActionResult Results(string query, bool byMain, bool byAlt, bool byScore)
        {
            var users = _applicationUserService.SearchUsers(query, byMain, byAlt, byScore);

            var model = new UserSearchModel
            {
                Results = BuildUserList(users),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Update(UpdateViewModel viewModel)
        {
            var currentUserID = _userManager.GetUserId(User);
            if (viewModel.UserID != currentUserID)
            {
                return;
            }

            if (ModelState.IsValid)
            {
                var new_main = _characterService.GetByID(viewModel.MainID);
                var new_alt = _characterService.GetByID(viewModel.AltID);
                _applicationUserService.UpdateUserCharacters(viewModel.UserID, new_main, new_alt);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddFriend(AddFriendViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = _applicationUserService.GetUser(viewModel.UserID, social: true);
                var newFriend = _applicationUserService.GetUser(viewModel.FriendID, social: true);
                _friendService.AddFriend(user, newFriend);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AcceptFriend(int requestID)
        {
            _friendService.AcceptFriend(requestID);
        }

        public string CurrentUserShortName()
        {
            var id = _userManager.GetUserId(User);
            var user = _applicationUserService.GetUser(id);
            return user.ShortName ?? user.UserName;
        }

        private IEnumerable<UserListingModel> BuildUserList(IEnumerable<ApplicationUser> users)
        {
            return users
                .Select(u => new UserListingModel
                {
                    ID = u.Id,
                    Name = u.UserName.Substring(0, u.UserName.IndexOf('@')),
                    Joined = u.MemberSince.ToString(),
                    MainName = u.Main == null ? "Random" : u.Main.Name,
                    MainImage = u.Main == null ? "random" : u.Main.ImageName,
                    AltName = u.Alt == null ? "Random" : u.Alt.Name,
                    AltImage = u.Alt == null ? "random" : u.Alt.ImageName,
                });
        }

        private UserListingModel BuildUserData(ApplicationUser user)
        {
            return new UserListingModel
            {
                ID = user.Id,
                Name = user.UserName.Substring(0, user.UserName.IndexOf('@')),
                MainName = user.Main == null ? "Random" : user.Main.Name,
                MainImage = user.Main == null ? "random" : user.Main.ImageName,
            };
        }

        private ProfileIndexModel BuildUserProfile(ApplicationUser user)
        {
            var updateViewModel = new UpdateViewModel
            {
                UserID = user.Id,
                MainID = user.Main?.ID,
                AltID = user.Alt?.ID,
            };
            var currentUserID = _userManager.GetUserId(User);
            return new ProfileIndexModel
            {
                ID = user.Id,
                CurrentUserID = currentUserID,
                CurrentUserIsFriends = user.Friends.Where(f => f.Id == currentUserID).Any(),
                Name = user.ShortName ?? user.UserName,
                Main = BuildCharacterData(user.Main),
                Alt = BuildCharacterData(user.Alt),
                PartnerName = user.Partner?.UserName,
                PartnerMainImage = user.Partner?.Main.ImageName,
                Friends = BuildFriendListing(user.FriendsApproved, user.FriendRequestsSent, user.FriendRequestsReceived),
                Comments = BuildCommentListing(user.Comments),
                Votes = BuildVoteListing(user.Votes),
                UpdateViewModel = updateViewModel,
            };
        }

        private IEnumerable<CommentDataModel> BuildCommentListing(IEnumerable<Comment> comments)
        {
            return comments
                .Select(c => new CommentDataModel
                {
                    Comment = c,
                    Title = c.Title,
                    Content = c.Content,
                    Created = c.Created.ToString(),
                    PosterName = c.Poster.UserName.Substring(0, c.Poster.UserName.IndexOf('@')),
                    Replies = BuildCommentListing(c.Replies),
                })
                .OrderByDescending(c => c.Created);
        }

        private FriendListingModel BuildFriendListing(ICollection<ApplicationUser> friendsApproved,
                                                      ICollection<Friend> friendRequestsSent,
                                                      ICollection<Friend> friendRequestsReceived)
        {
            var approvedFriends = friendsApproved
                .Select(f => new UserFriendModel
                {
                    FriendData = BuildUserData(f),
                });
            var requestedFriends = friendRequestsSent
                .Select(f => new UserFriendModel
                {
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedTo),
                });
            var friendRequests = friendRequestsReceived
                .Select(f => new UserFriendModel
                {
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedBy),
                });

            return new FriendListingModel
            {
                ApprovedFriends = approvedFriends,
                RequestedFriends = requestedFriends,
                FriendRequests = friendRequests,
            };

        }

        private IEnumerable<VoteDataModel> BuildVoteListing(IEnumerable<Vote> votes)
        {
            return votes
                .Select(v => new VoteDataModel
                {
                    Created = v.Created.ToString(":d"),
                    UserID = v.VoterID,
                    UserName = v.Voter.UserName.Substring(0, v.Voter.UserName.IndexOf('@')),
                    MostDifficult = BuildCharacterData(v.MostDifficult),
                    LeastDifficult = BuildCharacterData(v.LeastDifficult),
                    FlavorOfTheMonth = BuildCharacterData(v.FlavorOfTheMonth),
                    MostPowerful = BuildCharacterData(v.MostPowerful),
                });
        }

        private CharacterDataModel BuildCharacterData(Character c)
        {
            return new CharacterDataModel
            {
                ID = c.ID,
                Name = c == null ? "Random" : c.Name,
                ImageName = c == null ? "random" : c.ImageName,
            };
        }
    }
}