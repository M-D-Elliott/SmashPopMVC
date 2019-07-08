using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Controllers.Attributes.Validators;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Friend;
using System;

namespace SmashPopMVC.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriend _friendService;
        private readonly IApplicationUser _applicationUserService;

        public FriendController(IFriend friendService, IApplicationUser applicationUserService)
        {
            _friendService = friendService;
            _applicationUserService = applicationUserService;
        }

        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(AddFriendViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var existingFriend = _friendService.GetByUsersID(viewModel.CurrentUserID, viewModel.ProfileUserID);
                if(existingFriend == null)
                {
                    var user = _applicationUserService.Get(viewModel.CurrentUserID, social: true);
                    var newFriend = _applicationUserService.Get(viewModel.ProfileUserID, social: true);
                    _friendService.AddFriend(user, newFriend);
                    return Json(new { success = true, responseText = "Friend request sent!" });
                }
            }
            return Json(new { success = false, responseText = "Submitted data was incorrect." });
        }

        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Accept(int requestID)
        {
            var friendship = _friendService.Get(requestID);
            if (friendship.RequestFlag < RequestFlag.Approved)
            {
                friendship.RequestFlag = RequestFlag.Approved;
                _friendService.Update(friendship);
                return Json(new { success = true, responseText = "Friend accepted!" });
            }
            else
            {
                return Json(new { success = false, responseText = "Request already accepted." });
            }
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 1, Name = "CancelPartner", Message = "cancel partnerships")]
        public IActionResult CancelPartnership(RequestPartnershipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var friendship = _friendService.Get(model.FriendID, includeUsers: true);
                if (friendship != null && friendship.RequestFlag == RequestFlag.Partnered)
                {
                    var user1 = friendship.RequestedBy;
                    var user2 = friendship.RequestedTo;
                    if(user1.Id == model.CurrentUserID || user2.Id == model.CurrentUserID)
                    {
                        friendship.RequestFlag = RequestFlag.Approved;
                        friendship.ApprovalDate = DateTime.Now;
                        _friendService.Update(friendship);
                        user2.Partner = null;
                        user1.Partner = null;
                        _applicationUserService.Update(user1);
                        _applicationUserService.Update(user2);
                        return Json(new { success = true, responseText = "Partnership cancelled!" });
                    }
                }
            }
            return Json(new { success = false, responseText = "How did you get here?" });
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 5, Name = "RequestPartner", Message = "request partnerships")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult RequestPartnership(RequestPartnershipViewModel model)
        {
            if(ModelState.IsValid)
            {
                var friendship = _friendService.Get(model.FriendID, includeUsers: true);
                if (friendship != null)
                {
                    var flag = friendship.RequestFlag;
                    var user1 = friendship.RequestedBy;
                    var user2 = friendship.RequestedTo;
                    if (user1.Partner != null || user2.Partner != null)
                    {
                        return Json(new { success = false, responseText = "You each must clear your current partnerships." });
                    }
                    else
                    {
                        if (flag == RequestFlag.Approved)
                        {
                            if (model.CurrentUserID == user2.Id)
                            {
                                friendship.RequestedTo = user1;
                                friendship.RequestedBy = user2;
                            }
                            else if (model.CurrentUserID != user1.Id)
                            {
                                return Json(new { success = false, responseText = "How did you get here?" });
                            }
                            friendship.RequestFlag = RequestFlag.PartnerRequest;
                            _friendService.Update(friendship);
                            return Json(new { success = true, responseText = "Partnership request sent!" });
                        }
                        else if (flag == RequestFlag.PartnerRequest)
                        {
                            if (user1.Id == model.CurrentUserID)
                            {
                                return Json(new { success = false, responseText = "You already have a pending partnership with this user." });
                            }
                            else
                            {
                                friendship.RequestFlag = RequestFlag.Partnered;
                                friendship.ApprovalDate = DateTime.Now;
                                _friendService.Update(friendship);
                                user1.Partner = user2;
                                user1.Partner.Partner = user1;
                                _applicationUserService.Update(user1);
                                return Json(new { success = true, responseText = "Partner Accepted!" });
                            }
                        }
                        else if (flag == RequestFlag.Partnered)
                        {
                            return Json(new { success = false, responseText = "You are already partners with this user." });
                        }
                        else
                        {
                            return Json(new { success = false, responseText = "You must be friends with this user first." });
                        }
                    }
                }
            }
               
            return Json(new { success = false, responseText = "How did you get here?" });
        }
    }
}
