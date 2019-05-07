using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Services;

namespace SmashPopMVC.Controllers
{
    public class CharacterController : Controller
    {
        private readonly CharacterService _characterService;
        private readonly GameService _gameService;

        public CharacterController(CharacterService characterService, GameService gameService)
        {
            _characterService = characterService;
            _gameService = gameService;
        }

        public IActionResult Select(bool get_data = false)
        {
            var characters = _characterService.GetAll();
            IEnumerable<CharacterListingModel> characterList;
            if (get_data)
            {
                characterList = characters
                    .Select(c => new CharacterListingModel
                    {
                        ID = c.ID,
                        SmashID = c.SmashID,
                        Name = c.Name,
                        Tier = c.Tier,
                        OriginTitle = c.Origin.Title,
                        DebutDate = c.Origin.ReleaseDate.ToString(),
                        SmashDebutTitle = c.SmashOrigin.Title,
                        SmashDebutDate = c.SmashOrigin.ReleaseDate.ToString(),
                    });
            }
            else
            {
                characterList = characters
                    .Select(c => new CharacterListingModel
                    {
                        ID = c.ID,
                        SmashID = c.SmashID,
                        Name = c.Name,
                        Tier = c.Tier,
                    });
            }


            var model = new CharacterSelectingModel
            {
                CharacterList = characterList,
            };

            return View(model);
        }
    }
}