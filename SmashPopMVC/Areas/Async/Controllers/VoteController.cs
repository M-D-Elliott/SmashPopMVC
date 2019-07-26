using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Areas.Shared.Attributes.Validators;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Tally;
using SmashPopMVC.Models.Vote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Areas.Async.Controllers
{
    [Area("Async")]
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

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 30, Name = "ViewVoteHistory", Message = "view vote history")]
        [HttpGet]
        public async Task<IActionResult> Results(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var votes = await _voteService.GetByUser(id);
            var model = BuildVoteListing(votes);
            return View(model);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 5, Name = "CreateNewVote", Message = "create a new vote")]
        [HttpGet]
        public async Task<IActionResult> New(EditVoteModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }

            var now = DateTime.Now;
            var tally = await _tallyService.GetByDateCreatedOrCreate(now);

            model.Created = now;
            model.MostDifficult = BuildCharacterData(null);
            model.LeastDifficult = BuildCharacterData(null);
            model.FlavorOfTheMonth = BuildCharacterData(null);
            model.MostPowerful = BuildCharacterData(null);
            model.TallyData = BuildTallyData(tally);
            model.New = true;

            return View("Edit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EditVoteModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }

            var vote = (!ModelState.IsValid || model == null) ? null : new Vote
            {
                Voter = await _applicationUserService.Get(model.UserID),
                Created = model.Created,
                MostDifficult = _characterService.GetByIDOrRandom(model.MostDifficult.ID).Result,
                LeastDifficult = _characterService.GetByIDOrRandom(model.LeastDifficult.ID).Result,
                FlavorOfTheMonth = _characterService.GetByIDOrRandom(model.FlavorOfTheMonth.ID).Result,
                MostPowerful = _characterService.GetByIDOrRandom(model.MostPowerful.ID).Result,
                Tally = await _tallyService.Get(model.TallyData.ID),
            };
            return await AddObject(vote);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Day, Count = 5, Name = "AddNewVote", Message = "save a new vote")]
        [HttpPost]
        public async Task<IActionResult> AddObject(Vote vote)
        {
            if(vote == null)
            {
                return BadRequest("Vote does not exist.");
            }

            try
            {
                await _voteService.Add(vote);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
            return RedirectToAction("Profile", "ApplicationUser");
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Day, Count = 10, Name = "OpenVoteEdit", Message = "open vote editor")]
        [HttpGet]
        public async Task<IActionResult> Edit(EditVoteModel model)
        {
            var vote = await _voteService.Get(model.ID);
            var now = DateTime.Now;
            if(vote.Created.Month == now.Month && vote.Created.Year == now.Year)
            {
                model.MostDifficult = BuildCharacterData(vote.MostDifficult);
                model.LeastDifficult = BuildCharacterData(vote.LeastDifficult);
                model.FlavorOfTheMonth = BuildCharacterData(vote.FlavorOfTheMonth);
                model.MostPowerful = BuildCharacterData(vote.MostPowerful);
                model.TallyData = BuildTallyData(vote.Tally);
                model.New = false;
                return View(model);
            }
            else
            {
                return BadRequest("Vote is expired.");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EditVoteModel model)
        {
            if(ModelState.IsValid)
            {
                var vote = await _voteService.Get(model.ID);
                vote.MostDifficult = await _characterService.GetByIDOrRandom(model.MostDifficult.ID);
                vote.LeastDifficult = await _characterService.GetByIDOrRandom(model.LeastDifficult.ID);
                vote.FlavorOfTheMonth = await _characterService.GetByIDOrRandom(model.FlavorOfTheMonth.ID);
                vote.MostPowerful = await _characterService.GetByIDOrRandom(model.MostPowerful.ID);
                return await UpdateObject(model.ID, vote);
            }
            else
            {
                return BadRequest("Invalid Vote");
            }
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Day, Count = 10, Name = "SubmitVoteEdit", Message = "submit edited votes")]
        [HttpPost]
        public async Task<IActionResult> UpdateObject(int id, Vote vote)
        {
            if (vote == null || vote.ID != id)
            {
                return BadRequest();
            }

            try
            {
                await _voteService.Update(id, vote);
            }
            catch (Exception e)
            {
                return NotFound(e);
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
                    Created = v.Created.ToString("d"),
                    UserID = v.VoterID,
                    UserName = v.Voter?.UserName.Substring(0, v.Voter.UserName.IndexOf('@')),
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
                ID = c?.ID,
                Name = c == null ? "Random" : c.Name,
                ImageName = c == null ? "random" : c.ImageName,
            };
        }
    }
}
