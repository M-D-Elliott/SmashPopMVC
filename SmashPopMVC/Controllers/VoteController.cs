using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Character;
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

        public VoteController(IVote voteService, IApplicationUser userService)
        {
            _voteService = voteService;
        }

        [HttpGet]
        public IActionResult GetUserVotes(string userID)
        {
            var votes = _voteService.GetByUser(userID);

            var model = BuildVoteListing(votes);
            return GetVotes(model);
        }

        [HttpGet]
        public IActionResult GetVotes(VoteListingModel model)
        {
            return View(model);
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
