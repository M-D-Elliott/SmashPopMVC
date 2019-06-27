using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
{
    public interface IVote
    {
        Vote Get(int ID);
        IEnumerable<Vote> GetByUser(string userID, int take = 50);
        IEnumerable<Vote> GetByCharacter(int characterID, int take = 1000);

        void Update(Vote vote);
        void Add(Vote vote);
    }
}
