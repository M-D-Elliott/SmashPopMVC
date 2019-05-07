using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmashPopMVC.Services
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetUser(string id, bool friends = false)
        {
            var user = _context.ApplicationUsers.Where(u => u.Id == id);
            user = user
                .Include(u => u.Main)
                .Include(u => u.Alt);

            if(friends)
            {
                user = user
                    .Include(u => u.Friends).ThenInclude(fr => fr.Friend).ThenInclude(f => f.Main)
                    .Include(u => u.Partner).ThenInclude(p => p.Main);
            }

            return user.FirstOrDefault();
        }

        public IEnumerable<ApplicationUser> SearchUsers(
            string searchQuery, 
            bool byMain = false, 
            bool byAlt = false, 
            bool byScore = false)
        {
            return _context.ApplicationUsers.Where(u => u.UserName.Contains(searchQuery));
        }
    }
}
