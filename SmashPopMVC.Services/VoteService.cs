using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmashPopMVC.Service
{
    public class VoteService : IVote
    {
        private readonly ApplicationDbContext _context;

        public VoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Vote> Get()
        {
            return _context.Votes;
        }

        public IEnumerable<Vote> GetByUser(string userID, int take = 50)
        {
            return _context.Votes
                .Where(v => v.VoterID == userID)
                .Include(v => v.MostDifficult)
                .Include(v => v.LeastDifficult)
                .Include(v => v.FlavorOfTheMonth)
                .Include(v => v.MostPowerful)
                .Include(v => v.Voter)
                .Include(v => v.Tally)
                .OrderByDescending(v => v.Created)
                .Take(take);
        }

        public IEnumerable<Vote> GetByCharacter(int characterID, int take = 1000)
        {
            return _context.Votes
                .Where(v 
                    => v.MostDifficultID == characterID
                    || v.LeastDifficultID == characterID
                    || v.FlavorOfTheMonthID == characterID
                    || v.MostPowerfulID == characterID)
                .Include(v => v.MostDifficult)
                .Include(v => v.LeastDifficult)
                .Include(v => v.FlavorOfTheMonth)
                .Include(v => v.MostPowerful)
                .Include(v => v.Voter)
                .OrderByDescending(v => v.Created);
        }

        public void Add(Vote vote)
        {
            _context.Votes.Add(vote);
            _context.SaveChanges();
        }

    }
}
