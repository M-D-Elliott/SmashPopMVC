using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Controllers.Attributes.Validators;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Game;
using SmashPopMVC.Service;

namespace SmashPopMVC.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ICharacter _characterService;
        private readonly IGame _gameService;

        public CharacterController(ICharacter characterService, IGame gameService)
        {
            _characterService = characterService;
            _gameService = gameService;
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 50, Name = "SelectCharacters", Message = "use character selector")]
        [RequireHttps, HttpGet]
        public IActionResult Select()
        {
            var characters = _characterService.GetAll();

            var model = new CharacterListingModel
            {
                CharacterList = BuildCharacterListing(characters),
            };

            return PartialView(model);
        }

        public IActionResult Whack()
        {
            var characters =  _characterService.GetAllWithGames();

            var model = new CharacterDataListingModel
            {
                CharacterList = BuildCharacterData(characters),
            };

            return View(model);
        }

        private IEnumerable<CharacterSelectingModel> BuildCharacterListing(IEnumerable<Character> characters)
        {
            return characters
                .Select(c => new CharacterSelectingModel
                {
                    ID = c.ID,
                    SmashID = c.SmashID,
                    Name = c.Name,
                    Tier = c.Tier,
                    ImageName = c.ImageName,
                });
        }

        private IEnumerable<CharacterDataModel> BuildCharacterData(IEnumerable<Character> characters)
        {
            return characters
                .Select(c => new CharacterDataModel
                {
                    ID = c.ID,
                    SmashID = c.SmashID,
                    Name = c.Name,
                    Tier = c.Tier,
                    ImageName = c.ImageName,
                    Origin = c.Origin == null ? null : new GameDataModel
                    {
                        ID = c.Origin.ID,
                        Title = c.Origin.Title,
                        Release = c.Origin.ReleaseDate.ToString(),
                        Year = Int32.Parse(c.Origin.ReleaseDate.ToString("yy")),
                    },
                    SmashOrigin = new GameDataModel
                    {
                        ID = c.SmashOrigin.ID,
                        Title = c.SmashOrigin.SubTitle,
                        Release = c.SmashOrigin.ReleaseDate.ToString(),
                        Year = Int32.Parse(c.SmashOrigin.ReleaseDate.ToString("yy")),
                    },
                });
        }
    }
}
