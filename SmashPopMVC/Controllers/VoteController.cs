﻿using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Controllers.Attributes.Validators;
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

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 30, Name = "ViewVoteHistory", Message = "view vote history")]
        [RequireHttps, HttpGet]
        public IActionResult Results(string id)
        {
            var votes = _voteService.GetByUser(id);
            var model = BuildVoteListing(votes);
            return View(model);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 5, Name = "CreateNewVote", Message = "create a new vote")]
        [RequireHttps, HttpGet]
        public IActionResult New(EditVoteModel model)
        {
            var now = DateTime.Now;
            var tally = _tallyService.GetByDateCreatedOrCreate(now);

            model.Created = now;
            model.MostDifficult = BuildCharacterData(null);
            model.LeastDifficult = BuildCharacterData(null);
            model.FlavorOfTheMonth = BuildCharacterData(null);
            model.MostPowerful = BuildCharacterData(null);
            model.TallyData = BuildTallyData(tally);
            model.New = true;

            return View("Edit", model);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Day, Count = 5, Name = "AddNewVote", Message = "save a new vote")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(EditVoteModel model)
        {
            if (ModelState.IsValid)
            {
                var vote = new Vote
                {
                    Voter = _applicationUserService.Get(model.UserID),
                    Created = model.Created,
                    MostDifficult = _characterService.GetByIDOrRandom(model.MostDifficult.ID),
                    LeastDifficult = _characterService.GetByIDOrRandom(model.LeastDifficult.ID),
                    FlavorOfTheMonth = _characterService.GetByIDOrRandom(model.FlavorOfTheMonth.ID),
                    MostPowerful = _characterService.GetByIDOrRandom(model.MostPowerful.ID),
                    Tally = _tallyService.Get(model.TallyData.ID),
                };
                _voteService.Add(vote);
                return RedirectToAction("Profile", "ApplicationUser");
            }
            else
            {
                return Json(new { success = false, responseText = "Invalid vote." });
            }
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Day, Count = 10, Name = "OpenVoteEdit", Message = "open vote editor")]
        [RequireHttps, HttpGet]
        public IActionResult Edit(EditVoteModel model)
        {
            var vote = _voteService.Get(model.ID);
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
                return Json(new { success = false, responseText = "Vote is expired." });
            }
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Day, Count = 10, Name = "SubmitVoteEdit", Message = "submit edited votes")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(EditVoteModel model)
        {
            if(ModelState.IsValid)
            {
                var vote = _voteService.Get(model.ID);
                vote.MostDifficult = _characterService.GetByIDOrRandom(model.MostDifficult.ID);
                vote.LeastDifficult = _characterService.GetByIDOrRandom(model.LeastDifficult.ID);
                vote.FlavorOfTheMonth = _characterService.GetByIDOrRandom(model.FlavorOfTheMonth.ID);
                vote.MostPowerful = _characterService.GetByIDOrRandom(model.MostPowerful.ID);
                _voteService.Update(vote);
                return RedirectToAction("Profile", "ApplicationUser");
            }
            else
            {
                return Json(new { success = false, responseText = "Invalid vote." });
            }
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
                ID = c?.ID,
                Name = c == null ? "Random" : c.Name,
                ImageName = c == null ? "random" : c.ImageName,
            };
        }
    }
}
