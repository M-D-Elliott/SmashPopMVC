using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmashPopMVC.Service.Sync
{
    public class VoteService : IVote
    {
        private readonly ApplicationDbContext _context;

        public VoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Vote Get(int ID)
        {
            return _context.Votes
                .Where(v => v.ID == ID)
                .Include(v => v.MostDifficult)
                .Include(v => v.LeastDifficult)
                .Include(v => v.FlavorOfTheMonth)
                .Include(v => v.MostPowerful)
                .Include(v => v.Voter)
                .Include(v => v.Tally)
                .FirstOrDefault();
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
                .Take(take)
                .ToList();
        }

        public IEnumerable<Vote> GetByUser(string userID)
        {
            return GetByUser(userID, take: 50);
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
                .OrderByDescending(v => v.Created)
                .ToList();
        }

        public Vote Add(Vote vote)
        {
            var voteReturnValue = _context.Votes.Add(vote);
            _context.SaveChanges();
            return voteReturnValue.Entity;
        }

        public Vote Update(int id, Vote vote)
        {
            if (!_context.Votes.Any(v => v.ID == id))
            {
                throw new InvalidOperationException();
            }
            var voteReturnValue = _context.Votes.Update(vote);
            _context.SaveChanges();
            return voteReturnValue.Entity;
        }
    }
}
