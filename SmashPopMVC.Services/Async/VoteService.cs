using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Service.Async
{
    public class VoteService : IVote
    {
        private readonly ApplicationDbContext _context;

        public VoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Vote> Get(int ID)
        {
            return await _context.Votes
                .Where(v => v.ID == ID)
                .Include(v => v.MostDifficult)
                .Include(v => v.LeastDifficult)
                .Include(v => v.FlavorOfTheMonth)
                .Include(v => v.MostPowerful)
                .Include(v => v.Voter)
                .Include(v => v.Tally)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Vote>> GetByUser(string userID, int take = 50)
        {
            return await _context.Votes
                .Where(v => v.VoterID == userID)
                .Include(v => v.MostDifficult)
                .Include(v => v.LeastDifficult)
                .Include(v => v.FlavorOfTheMonth)
                .Include(v => v.MostPowerful)
                .Include(v => v.Voter)
                .Include(v => v.Tally)
                .OrderByDescending(v => v.Created)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vote>> GetByUser(string userID)
        {
            return await GetByUser(userID, take: 50);
        }

        public async Task<IEnumerable<Vote>> GetByCharacter(int characterID, int take = 1000)
        {
            return await _context.Votes
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
                .ToListAsync();
        }

        public async Task<Vote> Add(Vote vote)
        {
            var voteReturnValue = _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return voteReturnValue.Entity;
        }

        public async Task<Vote> Update(int id, Vote vote)
        {
            if (!_context.Votes.Any(v => v.ID == id))
            {
                throw new InvalidOperationException();
            }
            var voteReturnValue = _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
            return voteReturnValue.Entity;
        }
    }
}
