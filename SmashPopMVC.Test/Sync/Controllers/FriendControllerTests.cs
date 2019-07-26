﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Areas.Sync.Controllers;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Friend;
using System;
using SmashPopMVC.Extensions;

namespace SmashPopMVC.UnitTest.Sync.Controllers
{
    [TestFixture]
    public class FriendControllerTests
    {
        public ApplicationUser UserFactory() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-rgrg",
            UserName = "Yo",
        };

        public ApplicationUser UserFactory2() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-2grg",
            UserName = "Lol",
        };

        public Friend FriendFactory() => new Friend
        {
            ID = 100,
            RequestedBy = UserFactory(),
            RequestedTo = UserFactory2(),
            RequestTime = DateTime.Now,
            ApprovalDate = DateTime.Now,
            RequestFlag = RequestFlag.None,
        };

        [Test]
        public void Add_GoodUsersPassed_ReturnsJsonWithTrueSuccess()
        {
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.GetByUsersID(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns((Friend)null);

            var user1 = UserFactory();
            var user2 = UserFactory2();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            applicationUserServiceMock
                .SetupSequence(us => us.Get(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(user1)
                .Returns(user2);

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.Add(new AddFriendViewModel { CurrentUserID = user1.Id, ProfileUserID = user2.Id });

            friendServiceMock.Verify(fs => fs.GetByUsersID(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void Add_NullPassed_ReturnsJsonWithFalseSuccess()
        {
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.GetByUsersID(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns((Friend)null);

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.Add(new AddFriendViewModel { CurrentUserID = UserFactory().Id, ProfileUserID = null });

            friendServiceMock.Verify(fs => fs.GetByUsersID(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Add_FriendAlreadyExists_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.GetByUsersID(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(friend);

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.Add(new AddFriendViewModel { CurrentUserID = UserFactory().Id, ProfileUserID = null });

            friendServiceMock.Verify(fs => fs.GetByUsersID(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Add_ModelInvalid_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            var friendServiceMock = new Mock<IFriend>();

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            friendController.ModelState.AddModelError("Users", "Required");
            var result = friendController.Add(new AddFriendViewModel { CurrentUserID = UserFactory().Id, ProfileUserID = null });

            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Accept_GoodFriendIDPassed_ReturnsJsonWithTrueSuccess()
        {
            var friend = FriendFactory();
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.Accept(friend.ID);

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void Accept_NullFriendIDPassed_ReturnsJsonWithFalseSuccess()
        {
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(null, It.IsAny<bool>())).Returns((Friend)null);

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.Accept(null);

            friendServiceMock.Verify(fs => fs.Get(null, It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void Accept_RequestAlreadyApproved_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Approved;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.Accept(friend.ID);

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void CancelPartnership_PartnershipExists_ReturnsJsonWithTrueSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Partnered;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));
            friendServiceMock.Setup(fs => fs.Update(It.IsAny<int>(), It.Is<Friend>(f => f == friend))).Returns((friend));

            var user1 = UserFactory();
            var user2 = UserFactory2();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            applicationUserServiceMock
                .SetupSequence(us => us.Update(It.IsAny<string>(), It.IsAny<ApplicationUser>()))
                .Returns(user1)
                .Returns(user2);

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.CancelPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = user1.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            friendServiceMock.Verify(fs => fs.Update(It.IsAny<int>(), It.Is<Friend>( f => f == friend)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void CancelPartnership_ModelInvalid_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            var friendServiceMock = new Mock<IFriend>();

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            friendController.ModelState.AddModelError("Users", "Required");
            var result = friendController.CancelPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = "111" });

            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void CancelPartnership_NullFriendIDPassed_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Partnered;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(null, It.IsAny<bool>())).Returns((Friend)null);

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.CancelPartnership(new RequestPartnershipViewModel { FriendID = null, CurrentUserID = "1112" });

            friendServiceMock.Verify(fs => fs.Get(null, It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void RequestPartnership_FriendRequestApprovedNoPartnerships_ReturnsJsonWithTrueSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Approved;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));
            friendServiceMock.Setup(fs => fs.Update(It.IsAny<int>(), It.Is<Friend>(f => f == friend))).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedBy.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            friendServiceMock.Verify(fs => fs.Update(It.IsAny<int>(), It.Is<Friend>(f => f == friend)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void RequestPartnership_CurrentUserReceivedPartnerRequest_ReturnsJsonWithTrueSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.PartnerRequest;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));
            friendServiceMock.Setup(fs => fs.Update(It.IsAny<int>(), It.Is<Friend>(f => f == friend))).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedTo.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            friendServiceMock.Verify(fs => fs.Update(It.IsAny<int>(), It.Is<Friend>(f => f == friend)));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.True);
        }

        [Test]
        public void RequestPartnership_FriendIDNullPassed_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(null, It.IsAny<bool>())).Returns((Friend)null);

            var user1 = UserFactory();
            var user2 = UserFactory2();
            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.CancelPartnership(new RequestPartnershipViewModel { FriendID = null, CurrentUserID = user1.Id });

            friendServiceMock.Verify(fs => fs.Get(null, It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void RequestPartnership_AUserHasPartner_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Approved;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));

            friend.RequestedBy.Partner = friend.RequestedTo;
            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedBy.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void RequestPartnership_FriendshipBlockedOrLess_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Blocked;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedBy.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void RequestPartnership_CurrentUserMadePartnerRequest_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.PartnerRequest;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedBy.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void RequestPartnership_RequestFlagPartnered_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Partnered;
            var friendServiceMock = new Mock<IFriend>();
            friendServiceMock.Setup(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((friend));

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedBy.Id });

            friendServiceMock.Verify(fs => fs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public void RequestPartnership_ModelInvalid_ReturnsJsonWithFalseSuccess()
        {
            var friend = FriendFactory();
            friend.RequestFlag = RequestFlag.Approved;
            var friendServiceMock = new Mock<IFriend>();

            var applicationUserServiceMock = new Mock<IApplicationUser>();

            var friendController = new FriendController(friendServiceMock.Object, applicationUserServiceMock.Object);
            friendController.ModelState.AddModelError("Users", "Required");
            var result = friendController.RequestPartnership(new RequestPartnershipViewModel { FriendID = friend.ID, CurrentUserID = friend.RequestedBy.Id });

            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }
    }
}
