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

namespace SmashPopMVC.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _applicationUserService;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, IApplicationUser applicationUserService)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
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

            var user = _applicationUserService.GetUser(id, friends: true);

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
                _applicationUserService.UpdateUserCharacters(viewModel.UserID, viewModel.MainID, viewModel.AltID);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddFriend(AddFriendViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                _applicationUserService.AddFriend(viewModel.UserID, viewModel.FriendID);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AcceptFriend(int requestID)
        {
            _applicationUserService.AcceptFriend(requestID);
        }

        public string CurrentUserShortName()
        {
            var id = _userManager.GetUserId(User);
            var user = _applicationUserService.GetUser(id, friends: false);
            return user.ShortName ?? user.UserName;
        }

        private IEnumerable<UserListingModel> BuildUserList(IEnumerable<ApplicationUser> users)
        {
            return users
                .Select(u => new UserListingModel
                {
                    ID = u.Id,
                    Name = u.UserName,
                    Joined = u.MemberSince.ToString(),
                    MainName = u.Main == null ? "Random" : u.Main.Name,
                    MainImage = u.Main == null ? "random" : u.Main.ImageName,
                    AltName = u.Alt == null ? "Random" : u.Alt.Name,
                    AltImage = u.Alt == null ? "random" : u.Alt.ImageName,
                });
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
                MainName = user.Main == null ? "Random" : user.Main.Name,
                MainImage = user.Main == null ? "random" : user.Main.ImageName,
                AltName = user.Alt == null ? "Random" : user.Alt.Name,
                AltImage = user.Alt == null ? "random" : user.Alt.ImageName,
                PartnerName = user.Partner?.UserName,
                PartnerMainImage = user.Partner?.Main.ImageName,
                Friends = BuildFriendListing(user),
                UpdateViewModel = updateViewModel,
            };
        }

        private FriendListingModel BuildFriendListing(ApplicationUser user)
        {
            var approvedFriends = user.FriendsApproved
                .Select(f => new UserListingModel
                {
                    ID = f.Id,
                    Name = f.UserName.Substring(0, f.UserName.IndexOf('@')),
                    MainName = f.Main == null ? "Random" : f.Main.Name,
                    MainImage = f.Main == null ? "random" : f.Main.ImageName,
                });
            var requestedFriends = user.FriendRequestsSent
                .Select(f => new UserListingModel
                {
                    RequestID = f.ID,
                    ID = f.RequestedTo.Id,
                    Name = f.RequestedTo.UserName.Substring(0, f.RequestedTo.UserName.IndexOf('@')),
                    MainName = f.RequestedTo.Main == null ? "Random" : f.RequestedTo.Main.Name,
                    MainImage = f.RequestedTo.Main == null ? "random" : f.RequestedTo.Main.ImageName,
                });
            var friendRequests = user.FriendRequestsReceived
                .Select(f => new UserListingModel
                {
                    RequestID = f.ID,
                    ID = f.RequestedBy.Id,
                    Name = f.RequestedBy.UserName.Substring(0, f.RequestedBy.UserName.IndexOf('@')),
                    MainName = f.RequestedBy.Main == null ? "Random" : f.RequestedBy.Main.Name,
                    MainImage = f.RequestedBy.Main == null ? "random" : f.RequestedBy.Main.ImageName,
                });

            return new FriendListingModel
            {
                ApprovedFriends = approvedFriends,
                RequestedFriends = requestedFriends,
                FriendRequests = friendRequests,
            };

        }
    }
}