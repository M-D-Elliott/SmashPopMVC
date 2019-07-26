using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Areas.Sync.Controllers;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Vote;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.UnitTest.Sync.Controllers
{
    [TestFixture]
    public class VoteControllerTests
    {
        private DateTime now = DateTime.Now;

        public ApplicationUser UserFactory() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-rgrg",
            UserName = "Yo",
        };

        public Character CharacterFactory() => new Character
        {
            SmashID = "200",
            ID = 200,
            Name = "Tetris Piece",
            Tier = "G",
            Popularity = 1.00M,
            ImageName = "tetris-piece",
        };

        public Tally TallyFactory() => new Tally
        {
            Created = now,
            Month = now.ToString("MMMM"),
            Year = now.ToString("yyyy"),
        };

        public Vote VoteFactory() => new Vote
        {
            ID = 100,
            Voter = UserFactory(),
            Created = new DateTime(),
            Tally = TallyFactory(),
            MostDifficult = CharacterFactory(),
            LeastDifficult = CharacterFactory(),
            FlavorOfTheMonth = CharacterFactory(),
            MostPowerful = CharacterFactory(),
        };

        [Test]
        public void Results_NullUserID_ReturnsNotFound()
        {
            var voteServiceMock = new Mock<IVote>();
            voteServiceMock.Setup(vs => vs.GetByUser(It.IsAny<string>())).Returns((IEnumerable<Vote>)null);

            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();

            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);
            var result = voteController.Results(null);

            Assert.True(result is NotFoundResult);
        }

        [Test]
        public void Results_GoodUserID_ReturnsVotes()
        {
            var user = UserFactory();
            IEnumerable<Vote> vote = new List<Vote> { VoteFactory(), };

            var voteServiceMock = new Mock<IVote>();
            voteServiceMock.Setup(vs => vs.GetByUser(It.Is<string>(s=> s == user.Id))).Returns(vote);

            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();

            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);
            var result = voteController.Results(user.Id);

            Assert.True(result is ViewResult);
        }

        [Test]
        public void New_BlankEditModel_ReturnsViewResult()
        {
            var user = UserFactory();
            IEnumerable<Vote> vote = new List<Vote> { VoteFactory(), };
            var tally = TallyFactory();

            var voteServiceMock = new Mock<IVote>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();
            tallyServiceMock.Setup(ts => ts.GetByDateCreatedOrCreate(It.IsAny<DateTime>())).Returns(tally);

            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);
            var editVoteModel = new EditVoteModel();
            var result = voteController.New(editVoteModel);

            Assert.True(result is ViewResult);
            var model = (result as ViewResult).Model;
            Assert.True(model is EditVoteModel);
            Assert.AreEqual(model, editVoteModel);
        }

        [Test]
        public void Add_VotePassed_ReturnsRedirectToAction()
        {
            var vote = VoteFactory();
            var voteServiceMock = new Mock<IVote>();
            voteServiceMock.Setup(vs => vs.Add(It.Is<Vote>(v => v == vote))).Returns(vote);
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();

            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);
            var result = voteController.AddObject(vote);
            voteServiceMock.Verify(vs => vs.Add(It.Is<Vote>(v => v == vote)));
            Assert.True(result is RedirectToActionResult);
        }

        [Test]
        public void Add_NullPassed_BadResponseReturned()
        {
            var vote = VoteFactory();
            var voteServiceMock = new Mock<IVote>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();
            voteServiceMock.Setup(vs => vs.Add(It.Is<Vote>(v => v == vote))).Returns(vote);
            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);

            var result = voteController.AddObject((Vote)null);

            Assert.True(result is BadRequestResult);
        }

        [Test]
        public void Update_VotePassed_ReturnedVote()
        {
            var id = new int();
            var vote = new Vote
            {
                ID = id,
            };
            var voteServiceMock = new Mock<IVote>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();
            voteServiceMock.Setup(vs => vs.Update(It.IsAny<int>(), It.Is<Vote>(v => v == vote))).Returns(vote);
            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);

            var result = voteController.UpdateObject(id, vote);

            voteServiceMock.Verify(vs => vs.Update(It.IsAny<int>(), It.Is<Vote>(v => v == vote)));
            Assert.True(result is RedirectToActionResult);
        }

        [Test]
        public void Update_WrongIDPassed_ReturnsBadRequestObjectResult()
        {
            var vote = VoteFactory();
            var id = new int();

            var voteServiceMock = new Mock<IVote>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();
            voteServiceMock.Setup(vs => vs.Update(It.IsAny<int>(), It.Is<Vote>(v => v == vote))).Returns(vote);
            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);

            var result = voteController.UpdateObject(id, vote);
            
            Assert.True(result is BadRequestObjectResult);
        }

        public void Update_NullPassed_ReturnedVote()
        {
            var vote = VoteFactory();
            var id = new int();

            var voteServiceMock = new Mock<IVote>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();
            voteServiceMock.Setup(vs => vs.Update(It.IsAny<int>(), It.Is<Vote>(v => v == vote))).Returns(vote);
            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);

            var result = voteController.UpdateObject(id, null);

            Assert.True(result is BadRequestResult);
        }

        public void Update_ExceptionThrown_ReturnsNotFound()
        {
            var vote = VoteFactory();
            var id = new int();

            var voteServiceMock = new Mock<IVote>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var characterServiceMock = new Mock<ICharacter>();
            var tallyServiceMock = new Mock<ITally>();

            voteServiceMock
                .Setup(vs => vs.Update(It.IsAny<int>(), It.Is<Vote>(v => v == vote)))
                .Throws(new Exception());

            var voteController = new VoteController(voteServiceMock.Object, applicationUserServiceMock.Object, characterServiceMock.Object, tallyServiceMock.Object);

            var result = voteController.UpdateObject(id, vote);

            Assert.True(result is NotFoundResult);
        }
    }
}
