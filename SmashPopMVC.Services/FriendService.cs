using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using System;
using System.Linq;

namespace SmashPopMVC.Service
{
    public class FriendService : IFriend
    {
        private readonly ApplicationDbContext _context;

        public FriendService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Friend Get(int friendID, bool includeUsers = false)
        {
            var friend = _context.Friends
                .Where(f => f.ID == friendID);

            if(includeUsers)
            {
                friend = friend
                    .Include(f => f.RequestedBy)
                        .ThenInclude(u => u.Partner)
                    .Include(f=> f.RequestedTo)
                        .ThenInclude(u => u.Partner);
            }
            return friend.FirstOrDefault();
        }

        public void AddFriend(ApplicationUser user, ApplicationUser newFriend)
        {
            var friendRequest = new Friend()
            {
                RequestedBy = user,
                RequestedTo = newFriend,
                RequestTime = DateTime.Now,
                RequestFlag = RequestFlag.None
            };
            user.SentFriendRequests.Add(friendRequest);
            _context.SaveChanges();
        }

        public bool AcceptFriend(int friendID)
        {
            var friendship = Get(friendID);
            if(friendship.RequestFlag != RequestFlag.Approved)
            {
                friendship.RequestFlag = RequestFlag.Approved;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
