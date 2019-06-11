using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmashPopMVC.Service
{
    public class TallyService : ITally
    {
        private readonly ApplicationDbContext _context;

        public TallyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Tally Get(int ID)
        {
            return _context.Tallies
                .Where(t => t.ID == ID)
                .FirstOrDefault();
        }

        public IEnumerable<Tally> GetAll()
        {
            return _context.Tallies
                .Include(t => t.Votes);
        }

        public Tally GetByDateCreated(DateTime now)
        {
            return _context.Tallies
                .Where(t => t.Created.Month == now.Month && t.Created.Year == now.Year)
                .FirstOrDefault();
        }

        public Tally GetByDateCreatedOrCreate(DateTime now)
        {
            var tally = GetByDateCreated(now);
            if(tally == null)
            {
                var votePeriod = now.AddMonths(-1);
                tally = new Tally
                {
                    Created = now,
                    Month = votePeriod.ToString("MMMM"),
                    Year = votePeriod.ToString("yyyy"),
                };
                New(tally);
            }
            return tally;
        }

        public async void New(Tally tally)
        {
            _context.Add(tally);
            await _context.SaveChangesAsync();
        }
    }
}
