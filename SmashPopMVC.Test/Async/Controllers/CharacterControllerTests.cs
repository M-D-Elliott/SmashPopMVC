using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Areas.Async.Controllers;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.UnitTest.Async.Controllers
{
    [TestFixture]
    public class CharacterControllerTests
    {
        public SmashGame GameFactory() => new SmashGame
        {
            Title = "Tetris",
            SubTitle = "Blast",
            ReleaseDate = new DateTime(),
            Discriminator = "SmashGame",
        };

        public Character CharacterFactory() => new Character
        {
            SmashID = "200",
            ID = 200,
            Name = "Tetris Piece",
            Tier = "G",
            Popularity = 1.00M,
            SmashOrigin = GameFactory(),
            ImageName = "tetris-piece",
        };

        [Test]
        public async Task Select_NoCondition_ReturnsAllCharacters()
        {
            var characterServiceMock = new Mock<ICharacter>();
            IEnumerable<Character> characters = new List<Character> { CharacterFactory() };
            characterServiceMock.Setup(cs => cs.GetAll()).Returns(Task.FromResult(characters));
            var gameServiceMock = new Mock<IGame>();

            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);
            var result = await characterController.Select();

            characterServiceMock.Verify(cs => cs.GetAll());
            Assert.IsInstanceOf<PartialViewResult>(result);
            var viewResult = result as PartialViewResult;
            var model = viewResult.ViewData.Model as CharacterListingModel;
            Assert.IsAssignableFrom<List<CharacterDataModel>>(
                model.CharacterList);
        }

        [Test]
        public async Task Whack_NoCondition_ReturnsAllCharactersWithGames()
        {
            var characterServiceMock = new Mock<ICharacter>();
            IEnumerable<Character> characters = new List<Character> { CharacterFactory() };
            characterServiceMock.Setup(cs => cs.GetAllWithGames()).Returns(Task.FromResult(characters));
            var gameServiceMock = new Mock<IGame>();

            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = await characterController.Whack();

            characterServiceMock.Verify(cs => cs.GetAllWithGames());
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as CharacterListingModel;
            Assert.IsAssignableFrom<List<CharacterDataModel>>(
                model.CharacterList);
            Assert.IsAssignableFrom<GameDataModel>(
                model.CharacterList.First().SmashOrigin);
        }

        [Test]
        public async Task Get_IDPassed_ReturnsCharacter()
        {
            var character = CharacterFactory();

            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.GetByID(It.IsAny<int>())).Returns(Task.FromResult(character));
            var gameServiceMock = new Mock<IGame>();

            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = await characterController.Get(new int());

            Assert.True(result is ViewResult);

            Assert.AreEqual((result as ViewResult)?.Model, character);
        }

        [Test]
        public async Task Get_NoRequestedCharacter_ReturnsNotFound()
        {
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.GetByID(It.IsAny<int>())).Returns(Task.FromResult((Character)null));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = await characterController.Get(new int());

            Assert.True(result is NotFoundResult);
        }

        [Test]
        public async Task Add_CharacterPassed_ReturnsOk()
        {
            var character = new Character();
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.Add(It.Is<Character>(c => c == character)));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = await characterController.Add(character);
            characterServiceMock.Verify(cs => cs.Add(It.Is<Character>(c => c == character)));
            Assert.True(result is OkResult);
        }

        [Test]
        public async Task Add_NullPassed_ReturnsBadRequest()
        {
            Character character = null;
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.Add(It.Is<Character>(c => c == character)));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = await characterController.Add(character);
            Assert.True(result is BadRequestResult);
        }

        [Test]
        public async Task Delete_CharacterPassed_ReturnsOk()
        {
            var id = new int();
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.Delete(It.IsAny<int>()));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = await characterController.Delete(id);

            characterServiceMock.Verify(cs => cs.Delete(It.IsAny<int>()));
            Assert.True(result is OkResult);
        }
    }
}
