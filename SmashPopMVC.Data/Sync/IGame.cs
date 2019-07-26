using SmashPopMVC.Data.Models;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Sync
{
    public interface IGame
    {
        Game GetByID(int id);
        IEnumerable<Game> GetAll(string discriminator = null);
        IEnumerable<Game> GetOriginGames();
        IEnumerable<Game> GetSmashGames();

        void Add(Game game);
        void Delete(int id);
    }
}
