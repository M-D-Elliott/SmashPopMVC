using SmashPopMVC.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface ICharacter
    {
        Task<Character> GetByID(int? id);
        Task<IEnumerable<Character>> GetAll();
        Task<IEnumerable<Character>> GetAllWithGames();
        Task<Character> GetByIDOrRandom(int? id);

        Task<Character> Add(Character character);
        Task<Character> Update(Character character);
        Task Delete(int? id);
    }
}
