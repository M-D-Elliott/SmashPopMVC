using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Tally;
using SmashPopMVC.Models.Vote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Controllers
{
    public class VoteController : Controller
    {
        private readonly IVote _voteService;
        private readonly IApplicationUser _applicationUserService;
        private readonly ICharacter _characterService;
        private readonly ITally _tallyService;

        public VoteController(IVote voteService, IApplicationUser applicationUserService, ICharacter characterService, ITally tallyService)
        {
            _voteService = voteService;
            _applicationUserService = applicationUserService;
            _characterService = characterService;
            _tallyService = tallyService;
        }

        [HttpGet]
        public IActionResult GetUserVotes(string userID)
        {
            var votes = _voteService.GetByUser(userID);

            var model = BuildVoteListing(votes);
            return Results(model);
        }

        [HttpGet]
        public IActionResult Results(VoteListingModel model)
        {
            return View(model);
        }
        
        [HttpGet]
        public IActionResult New(NewVoteModel model)
        {
            var now = DateTime.Now;
            var tally = _tallyService.GetByDateCreatedOrCreate(now);

            model.Created = now;
            model.MostDifficult = null;
            model.LeastDifficult = null;
            model.FlavorOfTheMonth = null;
            model.MostPowerful = null;
            model.TallyData = BuildTallyData(tally);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(NewVoteModel model)
        {
            var vote = new Vote
            {
                Voter = _applicationUserService.GetUser(model.UserID),
                Created = model.Created,
                MostDifficult = _characterService.GetByIDOrRandom(model.MostDifficult.ID),
                LeastDifficult = _characterService.GetByIDOrRandom(model.LeastDifficult.ID),
                FlavorOfTheMonth = _characterService.GetByIDOrRandom(model.FlavorOfTheMonth.ID),
                MostPowerful = _characterService.GetByIDOrRandom(model.MostPowerful.ID),
                Tally = _tallyService.Get(model.TallyData.ID),
            };
            if (ModelState.IsValid)
            {
                _voteService.Add(vote);
            }
            return RedirectToAction("Profile", "ApplicationUser");
        }

        private TallyDataModel BuildTallyData(Tally tally)
        {
            return new TallyDataModel
            {
                ID = tally.ID,
                Month = tally.Month,
                Year = tally.Year,
            };
        }

        private VoteListingModel BuildVoteListing(IEnumerable<Vote> votes)
        {
            var results = votes
                .Select(v => new VoteDataModel
                {
                    Created = v.Created.ToString(":d"),
                    UserID = v.VoterID,
                    UserName = v.Voter.UserName.Substring(0, v.Voter.UserName.IndexOf('@')),
                    MostDifficult = BuildCharacterData(v.MostDifficult),
                    LeastDifficult = BuildCharacterData(v.LeastDifficult),
                    FlavorOfTheMonth = BuildCharacterData(v.FlavorOfTheMonth),
                    MostPowerful = BuildCharacterData(v.MostPowerful),
                });

            return new VoteListingModel
            {
                Results = results,
            };
        }

        private CharacterDataModel BuildCharacterData(Character c)
        {
            return new CharacterDataModel
            {
                ID = c.ID,
                Name = c == null ? "Random" : c.Name,
                ImageName = c == null ? "random" : c.ImageName,
            };
        }
    }
}
