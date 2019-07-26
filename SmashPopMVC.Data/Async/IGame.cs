using SmashPopMVC.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface IGame
    {
        Game GetByID(int id);
        IEnumerable<Game> GetAll(string discriminator = null);
        IEnumerable<Game> GetOriginGames();
        IEnumerable<Game> GetSmashGames();

        Task Add(Game game);
        Task Delete(int id);
    }
}
