using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface IVote
    {
        Task<Vote> Get(int ID);
        Task<IEnumerable<Vote>> GetByUser(string userID, int take = 50);
        Task<IEnumerable<Vote>> GetByUser(string userID);
        Task<IEnumerable<Vote>> GetByCharacter(int characterID, int take = 1000);

        Task<Vote> Update(int id, Vote vote);
        Task<Vote> Add(Vote vote);
    }
}
