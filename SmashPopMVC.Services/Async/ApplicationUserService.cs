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
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> Get(string id, bool social = false)
        {
            var user = _context.ApplicationUsers.Where(u => u.Id == id);
            user = user
                .Include(u => u.Main)
                .Include(u => u.Alt);

            if (social)
            {
                user = user
                    .Include(u => u.SentFriendRequests).ThenInclude(s => s.RequestedTo).ThenInclude(f => f.Main)
                    .Include(u => u.ReceievedFriendRequests).ThenInclude(s => s.RequestedBy).ThenInclude(f => f.Main)
                    .Include(u => u.Partner).ThenInclude(p => p.Alt)
                    .Include(u => u.Votes).ThenInclude(v => v.LeastDifficult)
                    .Include(u => u.Votes).ThenInclude(v => v.MostDifficult)
                    .Include(u => u.Votes).ThenInclude(v => v.FlavorOfTheMonth)
                    .Include(u => u.Votes).ThenInclude(v => v.MostPowerful);
            }

            return await user.FirstOrDefaultAsync();
        }


        public async Task<List<ApplicationUser>> SearchUsers(
            string searchQuery, 
            bool byMain,
            bool byAlt,
            bool byScore)
        {
            var users = string.IsNullOrEmpty(searchQuery)
                ? _context.ApplicationUsers
                    .Include(u => u.Main)
                    .Include(u => u.Alt)
                : _context.ApplicationUsers
                    .Include(u => u.Main)
                    .Include(u => u.Alt)
                    .Where(u
                        => byMain ? u.Main.Name.Contains(searchQuery)
                           : byAlt ? u.Alt.Name.Contains(searchQuery)
                           : byScore ? u.UserName.Contains(searchQuery)
                           : u.NormalizedUserName.Contains(searchQuery));
            return await users
                .OrderBy(u => u.NormalizedUserName)
                .ToListAsync();
        }

        public async Task<bool> ShortNameExists(string shortName)
        {
            return await _context.Users
                .Where(u => u.ShortName == shortName)
                .AnyAsync();
        }

        public async Task<ApplicationUser> Update(string id, ApplicationUser user)
        {
            if (!_context.ApplicationUsers.Any(u => u.Id == id))
            {
                throw new InvalidOperationException();
            }
            var userReturnValue = _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return userReturnValue.Entity;
        }
    }
}
