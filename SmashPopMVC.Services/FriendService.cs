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
            var friendship = _context.Friends
                .Where(f => f.ID == friendID);

            if(includeUsers)
            {
                friendship = friendship
                    .Include(f => f.RequestedBy)
                        .ThenInclude(u => u.Partner)
                    .Include(f=> f.RequestedTo)
                        .ThenInclude(u => u.Partner);
            }
            return friendship.FirstOrDefault();
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

        public void Update(Friend friendship)
        {
            _context.Entry(friendship).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Friend GetByUsersID(string user1ID, string user2ID, bool includeUsers = false)
        {
            var friendship = _context.Friends
                .Where(f => (user1ID == f.RequestedByID && user2ID == f.RequestedToID) || (user2ID == f.RequestedByID && user1ID == f.RequestedToID));

            if (includeUsers)
            {
                friendship = friendship
                    .Include(f => f.RequestedBy)
                        .ThenInclude(u => u.Partner)
                    .Include(f => f.RequestedTo)
                        .ThenInclude(u => u.Partner);
            }
            return friendship
                .FirstOrDefault();
        }
    }
}
