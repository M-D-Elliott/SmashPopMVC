using SmashPopMVC.Data.Models;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Sync
{
    public interface IVote
    {
        Vote Get(int ID);
        IEnumerable<Vote> GetByUser(string userID, int take = 50);
        IEnumerable<Vote> GetByUser(string userID);
        IEnumerable<Vote> GetByCharacter(int characterID, int take = 1000);

        Vote Update(int id, Vote vote);
        Vote Add(Vote vote);
    }
}
