using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Data.Sync
{
    public interface IFriend
    {
        Friend Get(int? friendID, bool includeUsers = false);
        Friend GetByUsersID(string user1ID, string user2ID, bool includeUsers = false);
        Friend AddFriend(ApplicationUser user, ApplicationUser newFriend);
        Friend Update(int id, Friend friendship);
    }
}
