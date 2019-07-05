using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Controllers.Attributes.Validators;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriend _friendService;

        public FriendController(IFriend friendService)
        {
            _friendService = friendService;
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 5, Name = "RequestPartner", Message = "request partnerships")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult RequestPartnership(int requestID)
        {
            var friendship = _friendService.Get(requestID);
            if(friendship.RequestFlag == RequestFlag.Approved)
            {
                if(friendship.RequestedBy.Partner != null || friendship.RequestedTo.Partner != null)
                {
                    return Json(new { success = false, responseText = "You each must clear your current partnerships." });
                }
                else
                {
                    
                    return Json(new { success = true, responseText = "Partnership sent!" });
                }
            }
            else
            {
                return Json(new { success = false, responseText = "You must add this person first." });
            }
        }

        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Accept(int requestID)
        {
            var unacceptedRequest = _friendService.AcceptFriend(requestID);
            if (unacceptedRequest)
            {
                return Json(new { success = true, responseText = "Friend accepted!" });
            }
            else
            {
                return Json(new { success = false, responseText = "Request already accepted." });
            }
        }
    }
}
