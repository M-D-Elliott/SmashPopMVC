using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using SmashPopMVC.Areas.Sync.Controllers;

namespace SmashPopMVC.UnitTest.Sync.Controllers
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
        public void Select_NoCondition_ReturnsAllCharacters()
        {
            var characterServiceMock = new Mock<ICharacter>();
            IEnumerable<Character> characters = new List<Character> { CharacterFactory() };
            characterServiceMock.Setup(cs => cs.GetAll()).Returns(characters);
            var gameServiceMock = new Mock<IGame>();

            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);
            var result = characterController.Select();

            characterServiceMock.Verify(cs => cs.GetAll());
            Assert.IsInstanceOf<PartialViewResult>(result);
            var viewResult = result as PartialViewResult;
            var model = viewResult.ViewData.Model as CharacterListingModel;
            Assert.IsAssignableFrom<List<CharacterDataModel>>(
                model.CharacterList);
        }

        [Test]
        public void Whack_NoCondition_ReturnsAllCharactersWithGames()
        {
            var characterServiceMock = new Mock<ICharacter>();
            IEnumerable<Character> characters = new List<Character> { CharacterFactory() };
            characterServiceMock.Setup(cs => cs.GetAllWithGames()).Returns(characters);
            var gameServiceMock = new Mock<IGame>();

            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = characterController.Whack();

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
        public void Get_IDPassed_ReturnsCharacter()
        {
            var character = CharacterFactory();

            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.GetByID(It.IsAny<int>())).Returns(character);
            var gameServiceMock = new Mock<IGame>();

            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = characterController.Get(new int());

            Assert.True(result is ViewResult);

            Assert.AreEqual((result as ViewResult)?.Model, character);
        }

        [Test]
        public void Get_NoRequestedCharacter_ReturnsNotFound()
        {
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.GetByID(It.IsAny<int>())).Returns((Character)null);
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = characterController.Get(new int());

            Assert.True(result is NotFoundResult);
        }

        [Test]
        public void Add_CharacterPassed_ReturnsOk()
        {
            var character = new Character();
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.Add(It.Is<Character>(c => c == character)));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = characterController.Add(character);
            characterServiceMock.Verify(cs => cs.Add(It.Is<Character>(c => c == character)));
            Assert.True(result is OkResult);
        }

        [Test]
        public void Add_NullPassed_ReturnsBadRequest()
        {
            Character character = null;
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.Add(It.Is<Character>(c => c == character)));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = characterController.Add(character);
            Assert.True(result is BadRequestResult);
        }

        [Test]
        public void Delete_CharacterPassed_ReturnsOk()
        {
            var id = new int();
            var characterServiceMock = new Mock<ICharacter>();
            characterServiceMock.Setup(cs => cs.Delete(It.IsAny<int>()));
            var gameServiceMock = new Mock<IGame>();
            var characterController = new CharacterController(characterServiceMock.Object, gameServiceMock.Object);

            var result = characterController.Delete(id);

            characterServiceMock.Verify(cs => cs.Delete(It.IsAny<int>()));
            Assert.True(result is OkResult);
        }
    }
}
