using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SmashPopMVC.Data.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using SmashPopMVC.Models.ApplicationUser;
using SmashPopMVC.Models.Vote;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Services.Sync;
using System;
using SmashPopMVC.Models.Friend;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Areas.Shared.Attributes.Validators;

namespace SmashPopMVC.Areas.Sync.Controllers
{
    [Area("Sync")]
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _applicationUserService;
        private readonly ICharacter _characterService;
        private readonly ICommentPackager _commentPackager;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, IApplicationUser applicationUserService, ICharacter characterService, ICommentPackager commentPackager)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _characterService = characterService;
            _commentPackager = commentPackager;
        }
        
        public IActionResult Profile(string id)
        {
            if (id == null)
            {
                id = GetCurrentUserID();
            }

            var user = _applicationUserService.Get(id, social: true);

            if (user != null)
            {
                var model = BuildUserProfile(user);

                return View(model);
            }
            return NotFound("User not found.");
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
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

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public IActionResult Results(UserSearchModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("How did you get here?");
            }
            else if(model == null)
            {
                return BadRequest("I know what you're doing. ;)");
            }
            else
            {
                if (model.Results == null)
                {
                    model = CompleteSearchModel(model);
                }
                return View(model);
            }
        }
        
        //[Throttle(Name = "UserUpdateThrottle", Seconds = 300)]
        [HttpPut, ValidateAntiForgeryToken]
        public IActionResult Update(UpdateViewModel viewModel)
        {
            var maxShortNameLength = 15;
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, responseText = "How did you get here?" });
            }

            if(viewModel == null)
            {
                return Json(new { success = false, responseText = "I know what you're doing." });
            }

            var currentUserID = GetCurrentUserID();
            if (viewModel.UserID != currentUserID)
            {
                return Json(new { success = false, responseText = "You are not this user." });
            }

            var user = _applicationUserService.Get(viewModel.UserID);
            if (user == null)
            {
                return Json(new { success = false, responseText = "User does not exist." });
            }

            bool update = false;
            if (user.Main?.ID != viewModel.MainID)
            {
                var new_main = _characterService.GetByID(viewModel.MainID);
                user.Main = new_main;
                update = true;
            }
            if (user.Alt?.ID != viewModel.AltID)
            {
                var new_alt = _characterService.GetByID(viewModel.AltID);
                user.Alt = new_alt;
                update = true;
            }
            var shortName = viewModel.ShortName;
            if(user.ShortName != shortName && shortName != null)
            {
                var length = shortName.Length;
                if (length > 0 && length < maxShortNameLength)
                {
                    var shortNameExists = _applicationUserService.ShortNameExists(shortName);
                    if (!shortNameExists)
                    {
                        user.ShortName = viewModel.ShortName;
                        update = true;
                    }
                }
            }

            if (update)
            {
                _applicationUserService.Update(user.Id, user);
                return Json(new { success = true, responseText = "Profile Updated!" });
            }
            else
            {
                return Json(new { success = false, responseText = "No changes detected." });
            }
        }

        private string GetCurrentUserID()
        {
            return _userManager.GetUserId(User);
        }

        private UserSearchModel CompleteSearchModel(UserSearchModel model, int take = 20, int page = 1)
        {
            var users = _applicationUserService.SearchUsers(model.SearchQuery, model.ByMain, model.ByAlt, model.ByScore);
            model.Take = model.Take == 0 ? take : model.Take;
            model.Page = model.Page == 0 ? page : model.Page;
            model.Results = BuildUserList(users, take: model.Take, page: model.Page);
            model.Total = model.Total == 0 ? users.Count() : model.Total;
            model.End = model.End ? model.End : model.Total <= model.Page * model.Take;
            return model;
        }

        private IEnumerable<UserListingModel> BuildUserList(List<ApplicationUser> users, int take = 20, int page = 1)
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
                    //AltName = u.Alt == null ? "Random" : u.Alt.Name,
                    //AltImage = u.Alt == null ? "random" : u.Alt.ImageName,
                })
                .ToList();
        }

        private ProfileIndexModel BuildUserProfile(ApplicationUser user)
        {
            var currentUserID = GetCurrentUserID();
            var isCurrentUser = user.Id == currentUserID;
            var friends = BuildFriendListing(user.SentFriendRequests, user.ReceievedFriendRequests, currentUserID, isCurrentUser: isCurrentUser);
            var partnerAlt = user.Partner?.Alt;

            return new ProfileIndexModel
            {
                ID = user.Id,
                IsCurrentUser = isCurrentUser,
                CurrentUserID = currentUserID,
                Name = user.ShortName ?? user.UserName,
                Main = BuildCharacterData(user.Main),
                Alt = BuildCharacterData(user.Alt),
                PartnerAltName = partnerAlt == null ? "Random" : partnerAlt.Name,
                PartnerAltImage = partnerAlt == null ? "random" : partnerAlt.ImageName,
                Joined = user.MemberSince.ToString("d"),
                Friends = friends,
                Comments = _commentPackager.BuildCommentListing(user.Id, currentUserID),
                Votes = BuildVoteListing(user.Votes, currentUserID, take: 3, isCurrentUser: user.Id == currentUserID),
                UpdateViewModel = BuildUpdateViewModel(user),
            };
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

        private FriendListingModel BuildFriendListing(
                                              ICollection<Friend> sentRequests,
                                              ICollection<Friend> receivedRequests,
                                              string currentUserID,
                                              bool isCurrentUser = false)
        {

            var approvedSentRequests = sentRequests
                .Where(x => x.Approved)
                .Select(f => new UserFriendModel {
                    Flag = f.RequestFlag,
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedTo),
                    Accepted = true,
                })
                .OrderByDescending(f => f.Flag)
                .ToList();

            var approvedReceivedRequests = receivedRequests
                .Where(x => x.Approved)
                .Select(f => new UserFriendModel
                {
                    Flag = f.RequestFlag,
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedBy),
                    Accepted = true,
                })
                .OrderByDescending(f => f.Flag);

            var approvedFriends = approvedSentRequests;
            approvedFriends.AddRange(approvedReceivedRequests);

            var partnerFriend = approvedFriends?
                .Where(af => af.Flag == RequestFlag.Partnered)
                .FirstOrDefault();

            var sentFriends = sentRequests
                .Where(f => !f.Approved)
                .Select(f => new UserFriendModel
                {
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedTo),
                    Accepted = true,
                })
                .ToList();
            var receivedFriends = receivedRequests
                .Where(f => !f.Approved)
                .Select(f => new UserFriendModel
                {
                    RequestID = f.ID,
                    FriendData = BuildUserData(f.RequestedBy),
                    Accepted = false,
                })
                .ToList();

            var friends = new List<UserFriendModel>();
            friends.AddRange(approvedFriends);
            friends.AddRange(sentFriends);
            friends.AddRange(receivedFriends);
            var currentUserFriend = friends?
                .Where(af => af.FriendData.ID == currentUserID)
                .FirstOrDefault();

            var requestPartnershipViewModel = (isCurrentUser || partnerFriend != null || currentUserFriend == null) ? null : new RequestPartnershipViewModel { CurrentUserID = currentUserID, FriendID = currentUserFriend.RequestID };

            return new FriendListingModel
            {
                ApprovedFriends = approvedFriends,
                RequestedFriends = sentFriends,
                FriendRequests = receivedFriends,
                PartnerFriend = partnerFriend ?? null,
                CurrentUserFriend = currentUserFriend ?? null,
                IsCurrentUser = isCurrentUser,
                RequestPartnershipViewModel = requestPartnershipViewModel,
            };
        }

        private VoteListingModel BuildVoteListing(IEnumerable<Vote> votes, string currentUserID, int take=1000, bool isCurrentUser=false)
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
                NewVoteModel = BuildNewVoteModel(currentUserID),
                IsCurrentUser = isCurrentUser,
            };
        }

        private EditVoteModel BuildNewVoteModel(string currentUserID)
        {
            return new EditVoteModel
            {
                UserID = currentUserID,
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