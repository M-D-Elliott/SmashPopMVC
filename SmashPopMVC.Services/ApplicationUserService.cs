using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Service
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetUser(string id, bool social = false)
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
                    .Include(u => u.Comments).ThenInclude(c => c.Replies)
                    .Include(u => u.Partner).ThenInclude(p => p.Main)
                    .Include(u => u.Votes);
            }

            return user.FirstOrDefault();
        }


        public IEnumerable<ApplicationUser> SearchUsers(
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
            return users.OrderBy(u => u.NormalizedUserName);
        }

        public void UpdateUserCharacters(string userID, Character new_main, Character new_alt)
        {
            var user = GetUser(userID);
            if (user != null)
            {
                if(user.Main?.ID != new_main?.ID)
                {
                    user.Main = new_main ?? null;
                    if(user.Main != null)
                    {
                        _context.Entry(user.Main).State = EntityState.Modified;
                    }
                }

                if (user.Alt?.ID != new_alt?.ID)
                {
                    user.Alt = new_alt ?? null;
                    if(user.Alt != null)
                    {
                        _context.Entry(user.Alt).State = EntityState.Modified;
                    }
                }
            }
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
