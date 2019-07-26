using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Service.Async
{
    public class FriendService : IFriend
    {
        private readonly ApplicationDbContext _context;

        public FriendService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Friend> Get(int? friendID, bool includeUsers = false)
        {
            Console.WriteLine("");
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
            return await friendship.FirstOrDefaultAsync();
        }

        public async Task<Friend> AddFriend(ApplicationUser user, ApplicationUser newFriend)
        {
            var friendRequest = new Friend()
            {
                RequestedBy = user,
                RequestedTo = newFriend,
                RequestTime = DateTime.Now,
                RequestFlag = RequestFlag.None
            };
            var friendReturnValue = _context.Friends.Add(friendRequest);
            await _context.SaveChangesAsync();
            return friendReturnValue.Entity;
        }

        public async Task<Friend> Update(int id, Friend friendship)
        {
            if (!_context.Friends.Any(v => v.ID == id))
            {
                throw new InvalidOperationException();
            }
            var friendReturnValue = _context.Friends.Update(friendship);
            await _context.SaveChangesAsync();
            return friendReturnValue.Entity;
        }

        public async Task<Friend> GetByUsersID(string user1ID, string user2ID, bool includeUsers = false)
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
            return await friendship
                .FirstOrDefaultAsync();
        }
    }
}
