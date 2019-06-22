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

        public void AddFriend(ApplicationUser user, ApplicationUser newFriend)
        {
            var friendRequest = new Friend()
            {
                RequestedBy = user,
                RequestedTo = newFriend,
                RequestTime = DateTime.Now,
                FriendRequestFlag = FriendRequestFlag.None
            };
            user.SentFriendRequests.Add(friendRequest);
            _context.SaveChanges();
        }

        public bool AcceptFriend(int friendID)
        {
            var friendship = _context.Friends.Where(f => f.ID == friendID).First();
            if(friendship.FriendRequestFlag != FriendRequestFlag.Approved)
            {
                friendship.FriendRequestFlag = FriendRequestFlag.Approved;
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
