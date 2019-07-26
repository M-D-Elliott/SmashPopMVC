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
    public class TallyService : ITally
    {
        private readonly ApplicationDbContext _context;

        public TallyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tally> Get(int ID)
        {
            return await _context.Tallies
                .Where(t => t.ID == ID)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Tally>> GetAll()
        {
            return await _context.Tallies
                .Include(t => t.Votes)
                .ToListAsync();
        }

        public async Task<Tally> GetByDateCreated(DateTime now)
        {
            return await _context.Tallies
                .Where(t => t.Created.Month == now.Month && t.Created.Year == now.Year)
                .FirstOrDefaultAsync();
        }

        public async Task<Tally> GetByDateCreatedOrCreate(DateTime now)
        {
            var tally = await GetByDateCreated(now);
            if(tally == null)
            {
                var votePeriod = now.AddMonths(-1);
                tally = new Tally
                {
                    Created = now,
                    Month = votePeriod.ToString("MMMM"),
                    Year = votePeriod.ToString("yyyy"),
                };
                await New(tally);
            }
            return await GetByDateCreated(now);
        }

        public async Task New(Tally tally)
        {
            _context.Add(tally);
            await _context.SaveChangesAsync();
        }
    }
}
