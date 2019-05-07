using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Models.UserProfile;
using SmashPopMVC.Services;
using Microsoft.AspNetCore.Identity;
using SmashPopMVC.Data.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SmashPopMVC.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUserService _applicationUserService;

        public UserProfileController(UserManager<ApplicationUser> userManager, ApplicationUserService applicationUserService)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var user = _applicationUserService.GetUser(userId, friends: true);

            var friends = user.Friends
                .Select(f => new FriendListingModel
                {
                    Name = f.Friend.UserName,
                    MainName = f.Friend.Main.Name,
                    MainImage = f.Friend.Main.ImageName,
                });


            var model = new ProfileIndexModel
            {
                Name = user.UserName,
                MainName = user.Main.Name,
                MainImage = user.Main.ImageName,
                AltName = user.Alt.Name,
                AltImage = user.Alt.ImageName,
                PartnerName = user.Partner.UserName,
                PartnerMainImage = user.Partner.Main.ImageName,
                Friends = friends,
            };

            return View(model);
        }
    }
}