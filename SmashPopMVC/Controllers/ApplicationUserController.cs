using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SmashPopMVC.Data.Models;
using System.Linq;
using SmashPopMVC.Data;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using SmashPopMVC.Models.ApplicationUser;
using SmashPopMVC.Models.Vote;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Services;
using System;
using SmashPopMVC.Controllers.Attributes.Validators;
using SmashPopMVC.Models.Friend;

namespace SmashPopMVC.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _applicationUserService;
        private readonly ICharacter _characterService;
        private readonly IFriend _friendService;
        private readonly ICommentPackager _commentPackager;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, 
                                         IApplicationUser applicationUserService, 
                                         ICharacter characterService, 
                                         IFriend friendService, 
                                         ICommentPackager commentPackager)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _characterService = characterService;
            _friendService = friendService;
            _commentPackager = commentPackager;
        }

        [RequireHttps]
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

        [RequireHttps, HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public IActionResult Search(string query, bool byMain, bool byAlt, bool byScore)
        {
            var model = new UserSearchModel
            {
                SearchQuery = query,
                ByMain = byMain,
                ByAlt = byAlt,
                ByScore = byScore,
            };
            model = CompleteSearchModel(model);
            if(model.Total == 1)
            {
                var user = model.Results.FirstOrDefault();
                return RedirectToAction("Profile", new { user.ID} );
            }
            return View("Results", model);
        }

        [RequireHttps, AllowAnonymous]
        public IActionResult Results(UserSearchModel model)
        {
            if(model.Results == null)
            {
                model = CompleteSearchModel(model);
            }
            return View(model);
        }
        
        [Throttle(Name = "UserUpdateThrottle", Seconds = 300)]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(UpdateViewModel viewModel)
        {
            var currentUserID = GetCurrentUserID();
            if (viewModel.UserID != currentUserID)
            {
                return Json(new { success = false, responseText = "You are not this user." });
            }

            if (ModelState.IsValid)
            {
                var new_main = _characterService.GetByID(viewModel.MainID);
                var new_alt = _characterService.GetByID(viewModel.AltID);
                _applicationUserService.UpdateUserCharacters(viewModel.UserID, new_main, new_alt);
                return Json(new { success = true, responseText = "Profile Updated!" });
            }
            else
            {
                return Json(new { success = false, responseText = "Error."});
            }
        }
        
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddFriend(AddFriendViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = _applicationUserService.GetUser(viewModel.UserID, social: true);
                var newFriend = _applicationUserService.GetUser(viewModel.FriendID, social: true);
                _friendService.AddFriend(user, newFriend);
                return Json(new { success = true, responseText = "Friend request sent!" });
            }
            return Json(new { success = false, responseText = "Submitted data was incorrect." });
        }

        public string CurrentUserShortName()
        {
            var id = _userManager.GetUserId(User);
            var user = _applicationUserService.GetUser(id);
            return user.ShortName ?? user.UserName;
        }

        private string GetCurrentUserID()
        {
            return _userManager.GetUserId(User);
        }

        private UserSearchModel CompleteSearchModel(UserSearchModel model)
        {
            var users = _applicationUserService.SearchUsers(model.SearchQuery, model.ByMain, model.ByAlt, model.ByScore);
            model.Take = model.Take == 0 ? 20 : model.Take;
            model.Page = model.Page == 0 ? 1 : model.Page;
            model.Results = BuildUserList(users, page: model.Page, take: model.Take);
            model.Total = model.Total == 0 ? users.Count() : model.Total;
            model.End = model.End ? model.End : model.Total <= model.Page * model.Take;
            return model;
        }

        private IEnumerable<UserListingModel> BuildUserList(IList<ApplicationUser> users, int page = 1, int take = 20)
        {
            return users
                .Skip(take * (page - 1))
                .Take(take)
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
            int maxNameLength = 5;
            return new UserListingModel
            {
                ID = user.Id,
                Name = user.ShortName.Length > maxNameLength ? user.ShortName.Substring(0, maxNameLength - 1) + "..." : user.ShortName,
                MainName = user.Main == null ? "Random" : user.Main.Name,
                MainImage = user.Main == null ? "random" : user.Main.ImageName,
            };
        }

        private ProfileIndexModel BuildUserProfile(ApplicationUser user)
        {
            var currentUserID = GetCurrentUserID();
            var currentUserFriend = user.Friends.Where(f => f.Id == currentUserID).ToList();
            var currentUserIsFriend = user.FriendsApproved.Where(f => f.Id == currentUserID).Any();
            return new ProfileIndexModel
            {
                ID = user.Id,
                IsCurrentUser = user.Id == currentUserID,
                CurrentUserID = currentUserID,
                CurrentUserFriend = BuildFriendModel(currentUserFriend, accepted: currentUserIsFriend).FirstOrDefault(),
                Name = user.ShortName ?? user.UserName,
                Main = BuildCharacterData(user.Main),
                Alt = BuildCharacterData(user.Alt),
                Joined = user.MemberSince.ToString("d"),
                PartnerName = user.Partner?.UserName,
                PartnerMainImage = user.Partner?.Main.ImageName,
                Friends = BuildFriendListing(user.FriendsApproved, user.FriendRequestsSent, user.FriendRequestsReceived, isCurrentUser: (currentUserID == user.Id)),
                Comments = _commentPackager.BuildCommentListing(user.Id, currentUserID),
                Votes = BuildVoteListing(user.Votes, take: 3, currentUser: user.Id == currentUserID),
                UpdateViewModel = BuildUpdateViewModel(user),
            };
        }

        private FriendListingModel BuildFriendListing(ICollection<ApplicationUser> friendsApproved,
                                                      ICollection<Friend> friendRequestsSent,
                                                      ICollection<Friend> friendRequestsReceived,
                                                      bool isCurrentUser = false)
        {
            var approvedFriends = BuildFriendModel(friendsApproved, accepted: true);

            var requestedFriends = friendRequestsSent
                .Select(f => new UserFriendModel
                {
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedTo),
                    Accepted = true,
                });
            var friendRequests = friendRequestsReceived
                .Select(f => new UserFriendModel
                {
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedBy),
                    Accepted = false,
                });

            return new FriendListingModel
            {
                ApprovedFriends = approvedFriends,
                RequestedFriends = requestedFriends,
                FriendRequests = friendRequests,
                IsCurrentUser = isCurrentUser,
            };

        }

        public IEnumerable<UserFriendModel> BuildFriendModel(ICollection<ApplicationUser> friends, bool accepted = false)
        {
            return friends
                .Select(f => new UserFriendModel
                {
                    FriendData = BuildUserData(f),
                    Accepted = accepted,
                });
        }

        private VoteListingModel BuildVoteListing(IEnumerable<Vote> votes, int take=1000, bool currentUser=false)
        {
            var now = DateTime.Now;
            votes = votes
                .OrderByDescending(v => v.Created)
                .Take(take);
            var results = votes
                .Select(v => new VoteDataModel
                {
                    ID = v.ID,
                    Editable = v.Created.Month == now.Month && v.Created.Year == now.Year,
                    Created = v.Created.ToString("d"),
                    UserID = v.VoterID,
                    UserName = v.Voter.UserName.Substring(0, v.Voter.UserName.IndexOf('@')),
                    MostDifficult = BuildCharacterData(v.MostDifficult),
                    LeastDifficult = BuildCharacterData(v.LeastDifficult),
                    FlavorOfTheMonth = BuildCharacterData(v.FlavorOfTheMonth),
                    MostPowerful = BuildCharacterData(v.MostPowerful),
                });

            var mostRecentVote = results.FirstOrDefault();
            var currentVote = mostRecentVote == null ? false : mostRecentVote.Editable;
            return new VoteListingModel
            {
                Results = results,
                CurrentVote = currentVote,
                NewVoteModel = BuildNewVoteModel(),
                IsCurrentUser = currentUser,
            };
        }

        private EditVoteModel BuildNewVoteModel()
        {
            return new EditVoteModel
            {
                UserID = GetCurrentUserID(),
            };
        }

        private UpdateViewModel BuildUpdateViewModel(ApplicationUser user)
        {
            return new UpdateViewModel
            {
                UserID = user.Id,
                MainID = user.Main?.ID,
                AltID = user.Alt?.ID,
            };
        }

        private CharacterDataModel BuildCharacterData(Character c)
        {
            return new CharacterDataModel
            {
                ID = c?.ID,
                Name = c == null ? "Random" : c.Name,
                ImageName = c == null ? "random" : c.ImageName,
            };
        }
    }
}