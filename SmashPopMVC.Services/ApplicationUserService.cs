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
        private readonly ICharacter _characterService;

        public ApplicationUserService(ApplicationDbContext context, ICharacter characterService)
        {
            _context = context;
            _characterService = characterService;
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
                    .Include(u => u.Partner).ThenInclude(p => p.Main);
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

        public void UpdateUserCharacters(string userID, int? mainID, int? altID)
        {
            var user = GetUser(userID);
            if (user != null)
            {
                var new_main = _characterService.GetByID(mainID);
                if(user.Main?.ID != new_main?.ID)
                {
                    user.Main = new_main;
                    if(user.Main != null)
                    {
                        _context.Entry(user.Main).State = EntityState.Modified;
                    }

                }

                var new_alt = _characterService.GetByID(altID);
                if (user.Alt?.ID != new_alt?.ID)
                {
                    user.Alt = new_alt;
                    if(user.Alt != null)
                    {
                        _context.Entry(user.Alt).State = EntityState.Modified;
                    }
                }
            }
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void AddFriend(string userID, string friendUserID)
        {
            var user = GetUser(userID, social: true);
            var new_friend = GetUser(friendUserID);
            var friendRequest = new Friend()
            {
                RequestedBy = user,
                RequestedTo = new_friend,
                RequestTime = DateTime.Now,
                FriendRequestFlag = FriendRequestFlag.None
            };
            user.SentFriendRequests.Add(friendRequest);
            _context.SaveChanges();
        }

        public void AcceptFriend(int friendID)
        {
            var friendship = _context.Friends.Where(f => f.ID == friendID).First();
            friendship.FriendRequestFlag = FriendRequestFlag.Approved;
            _context.SaveChanges();

        }
    }
}
