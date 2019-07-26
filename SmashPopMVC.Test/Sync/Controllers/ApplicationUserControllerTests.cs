using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Extensions;
using SmashPopMVC.Models.ApplicationUser;
using SmashPopMVC.Services.Sync;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using SmashPopMVC.Areas.Sync.Controllers;

namespace SmashPopMVC.UnitTest.Sync.Controllers
{
    [TestFixture]
    public class ApplicationUserControllerTests
    {
        public Character CharacterFactory() => new Character
        {
            SmashID = "200",
            ID = 200,
            Name = "Tetris Piece",
            Tier = "G",
            Popularity = 1.00M,
            ImageName = "tetris-piece",
        };

        public Character CharacterFactory2() => new Character
        {
            SmashID = "300",
            ID = 300,
            Name = "Poo",
            Tier = "G",
            Popularity = 1.00M,
            ImageName = "poo",
        };

        public ApplicationUser UserFactory() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-rgrg",
            UserName = "Yo@gmail.com",
            ShortName = "Yo"
        };

        public ApplicationUser UserFactory2() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-2grg",
            UserName = "Lol@gmail.com",
            ShortName = "Lol"
        };

        [Test]
        public void Profile_CurrentUserIDPassed_ReturnsViewWithModelContainingCurrentUserProfile()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);

            var result = controller.Profile(user1.Id);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is ViewResult);
            var model = (result as ViewResult).Model;
            Assert.True(model is ProfileIndexModel);
            var profileIndexModel = model as ProfileIndexModel;
            Assert.IsTrue(profileIndexModel.IsCurrentUser);
        }

        [Test]
        public void Profile_NullIDPassed_ReturnsViewWithModelContainingCurrentUserProfile()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);

            var result = controller.Profile(null);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is ViewResult);
            var model = (result as ViewResult).Model;
            Assert.IsAssignableFrom<ProfileIndexModel>(
                model);
            var profileIndexModel = model as ProfileIndexModel;
            Assert.IsTrue(profileIndexModel.IsCurrentUser);
        }

        [Test]
        public void Profile_ProfileUserIDPassed_ReturnsViewWithModelContainingUserProfile()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var character = CharacterFactory();
            var commentPackagerMock = new Mock<ICommentPackager>();
            
            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user2.Id), It.IsAny<bool>())).Returns(user2);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);

            var result = controller.Profile(user2.Id);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user2.Id), It.IsAny<bool>()));
            Assert.True(result is ViewResult);
            var model = (result as ViewResult).Model;
            Assert.IsAssignableFrom<ProfileIndexModel>(
                model);
            var profileIndexModel = model as ProfileIndexModel;
            Assert.IsFalse(profileIndexModel.IsCurrentUser);
        }

        [Test]
        public void Profile_UserIsNull_ReturnsNotFoundResult()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var character = CharacterFactory();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;

            applicationUserServiceMock.Setup(us => us.Get(It.IsAny<string>(), It.IsAny<bool>())).Returns((ApplicationUser)null);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);

            var result = controller.Profile(user2.Id);

            applicationUserServiceMock.Verify(us => us.Get(It.IsAny<string>(), It.IsAny<bool>()));
            Assert.That(result is NotFoundObjectResult);
        }

        [Test]
        public void Search_NullQueryPassed_ReturnsAllUsers()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();


            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;
            var userList = new List<ApplicationUser>
            {
                user1,
                user2,
            };
            
            applicationUserServiceMock.Setup(us => us.SearchUsers(It.Is<string>(s => s == null), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(userList);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);

            var result = controller.Search(null, false, false, false);

            applicationUserServiceMock.Verify(us => us.SearchUsers(It.Is<string>(s => s == null), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            Assert.True(result is ViewResult);
            var retModel = (result as ViewResult).Model;
            Assert.True(retModel is UserSearchModel);
            var results = (retModel as UserSearchModel).Results;
            Assert.True(results.Any(u => u.ID == user1.Id));
        }

        [Test]
        public void Search_NamePassedRelatedToOneUser_ReturnsUserProfile()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();


            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;
            var userList = new List<ApplicationUser>
            {
                user1,
            };

            applicationUserServiceMock.Setup(us => us.SearchUsers(It.Is<string>(s => s == user1.ShortName), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(userList);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);

            var result = controller.Search(user1.ShortName, false, false, false);

            applicationUserServiceMock.Verify(us => us.SearchUsers(It.Is<string>(s => s == user1.ShortName), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));
            Assert.True(result is RedirectToActionResult);
        }

        [Test]
        public void Results_NullResultsPassed_ReturnsViewWithNewModel()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();


            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = user1.Alt = user2.Main = user2.Alt = CharacterFactory();
            user1.Partner = user2;
            var userList = new List<ApplicationUser>
            {
                user1,
                user2,
            };

            applicationUserServiceMock.Setup(us => us.SearchUsers(It.Is<string>(s => s == null), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(userList);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var model = new UserSearchModel
            {
                SearchQuery = null,
                Results = null,
                ByMain = false,
                ByAlt = false,
                ByScore = false,
            };
            var result = controller.Results(model);

            applicationUserServiceMock.Verify(us => us.SearchUsers(It.Is<string>(s => s == null), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            Assert.True(result is ViewResult);
            var retModel = (result as ViewResult).Model;
            Assert.True(retModel is UserSearchModel);
        }

        [Test]
        public void Results_ModelWithResultsListPassed_ReturnsViewWithNewModel()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var model = new UserSearchModel
            {
                Results = new List<UserListingModel> { },
            };
            var result = controller.Results(model);

            Assert.True(result is ViewResult);
            var retModel = (result as ViewResult).Model;
            Assert.True(retModel is UserSearchModel);
        }

        [Test]
        public void Results_NullModelPAssed_ReturnsBadRequestObject()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Results(null);
            Assert.True(result is BadRequestObjectResult);
        }

        [Test]
        public void Results_ModelInvalidPassed_ReturnsBadRequestObject()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            controller.ModelState.AddModelError("stringint", "A string was passed as an integer.");
            var model = new UserSearchModel
            {
                Results = null,
            };
            var result = controller.Results(model);
            Assert.True(result is BadRequestObjectResult);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsMainAltSwap_ReturnsJsonWithSuccessTrue()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Alt.ID,
                AltID = user1.Main.ID,
                ShortName = user1.ShortName,
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            characterServiceMock.Setup(us => us.GetByID(It.Is<int>(i => i == model.MainID))).Returns(user1.Alt);
            characterServiceMock.Setup(us => us.GetByID(It.Is<int>(i => i == model.AltID))).Returns(user1.Main);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            applicationUserServiceMock.Setup(us => us.Update(It.Is<string>(s => s == user1.Id), It.Is<ApplicationUser>(u => u == user1))).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            characterServiceMock.Verify(us => us.GetByID(It.Is<int>(i => i == model.MainID)));
            characterServiceMock.Verify(us => us.GetByID(It.Is<int>(i => i == model.AltID)));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(us => us.Update(It.Is<string>(s => s == user1.Id), It.Is<ApplicationUser>(u => u == user1)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsCharactersNull_ReturnsJsonWithSuccessTrue()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = null,
                AltID = null,
                ShortName = user1.ShortName,
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            characterServiceMock.Setup(cs => cs.GetByID(It.Is<int?>(i => i == model.MainID))).Returns((Character)null);
            characterServiceMock.Setup(cs => cs.GetByID(It.Is<int?>(i => i == model.AltID))).Returns((Character)null);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            applicationUserServiceMock.Setup(us => us.Update(It.Is<string>(s => s == user1.Id), It.Is<ApplicationUser>(u => u == user1))).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            characterServiceMock.Verify(cs => cs.GetByID(It.Is<int?>(i => i == model.MainID)));
            characterServiceMock.Verify(cs => cs.GetByID(It.Is<int?>(i => i == model.AltID)));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(us => us.Update(It.Is<string>(s => s == user1.Id), It.Is<ApplicationUser>(u => u == user1)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsShortNameChange_ReturnsJsonWithSuccessTrue()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = "Teehee!",
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            applicationUserServiceMock.Setup(us => us.ShortNameExists(It.Is<string>(s => s == model.ShortName))).Returns(false);
            applicationUserServiceMock.Setup(us => us.Update(It.Is<string>(s => s == user1.Id), It.Is<ApplicationUser>(u => u == user1))).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(us => us.ShortNameExists(It.Is<string>(s => s == user1.ShortName)));
            applicationUserServiceMock.Verify(us => us.Update(It.Is<string>(s => s == user1.Id), It.Is<ApplicationUser>(u => u == user1)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsShortNameChangeShortNameTaken_ReturnsJsonWithSuccessFalse ()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = "Teehee!",
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            applicationUserServiceMock.Setup(us => us.ShortNameExists(It.Is<string>(s => s == model.ShortName))).Returns(true);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(us => us.ShortNameExists(It.Is<string>(s => s == model.ShortName)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsShortNameChangeShortTooShort_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = "",
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsShortNameChangeShortTooLong_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = "12345678910112131415",
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsShortNameChangeShortIsNull_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = null,
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_ValidUpdateViewModelUserExistsNoChanges_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = user1.ShortName,
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns(user1);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_InvalidViewModel_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = user1.ShortName,
            };

            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            controller.ModelState.AddModelError("Data", "Data is missing or invalid.");
            var result = controller.Update(model);

            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_NullViewModel_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(null);

            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_NotCurrentUser_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = user1.ShortName,
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user2.Id);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Update_UserDoesNotExist_ReturnsJsonWithSuccessFalse()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var user1 = UserFactory();
            var user2 = UserFactory2();
            user1.Main = CharacterFactory();
            user1.Alt = CharacterFactory2();

            var model = new UpdateViewModel
            {
                UserID = user1.Id,
                MainID = user1.Main.ID,
                AltID = user1.Alt.ID,
                ShortName = user1.ShortName,
            };

            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);
            applicationUserServiceMock.Setup(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>())).Returns((ApplicationUser)null);
            var controller = new ApplicationUserController(userManagerMock.Object,
                                                           applicationUserServiceMock.Object,
                                                           characterServiceMock.Object,
                                                           commentPackagerMock.Object);
            var result = controller.Update(model);

            userManagerMock.Verify(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()));
            applicationUserServiceMock.Verify(us => us.Get(It.Is<string>(s => s == user1.Id), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }
    }
}

