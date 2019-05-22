using SmashPopMVC.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmashPopMVC.Data
{
    public interface ICharacter
    {
        Character GetByID(int? id);
        IEnumerable<Character> GetAll();
        IEnumerable<Character> GetAllWithGames();

        Task Add(Character character);
        Task Delete(int id);
    }
}
