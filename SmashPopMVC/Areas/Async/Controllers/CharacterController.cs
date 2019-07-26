using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Areas.Shared.Attributes.Validators;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Game;

namespace SmashPopMVC.Areas.Async.Controllers
{
    [Area("Async")]
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
        [HttpGet]
        public async Task<IActionResult> Select()
        {
            var characters = await _characterService.GetAll();

            var model = new CharacterListingModel
            {
                CharacterList = BuildCharacterListing(characters),
            };

            return PartialView(model);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Whack()
        {
            var characters = await _characterService.GetAllWithGames();

            var model = new CharacterListingModel
            {
                CharacterList = BuildCharacterData(characters),
            };

            return View(model);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 50, Name = "GetCharacter", Message = "get a character")]
        [HttpGet]
        public async Task<IActionResult> Get(int id) {

            var character = await _characterService.GetByID(id);
            if(character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 50, Name = "GetCharacter", Message = "get a character")]
        [HttpGet]
        public async Task<IActionResult> Add(Character character)
        {
            if (character == null)
            {
                return BadRequest();
            }

            try
            {
                await _characterService.Add(character);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Ok();
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 50, Name = "DeleteCharacter", Message = "delete a character")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            try
            {
                await _characterService.Delete(id);
            }
            catch (Exception e)
            {
                return NotFound(e);
            }

            return Ok();
        }

        private IEnumerable<CharacterDataModel> BuildCharacterListing(IEnumerable<Character> characters)
        {
            return characters
                .Select(c => new CharacterDataModel
                {
                    ID = c.ID,
                    SmashID = c.SmashID,
                    Name = c.Name,
                    Tier = c.Tier,
                    ImageName = c.ImageName,
                })
                .ToList();
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
                })
                .ToList();
        }
    }
}
