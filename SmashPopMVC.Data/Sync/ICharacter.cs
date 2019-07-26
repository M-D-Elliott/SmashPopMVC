using SmashPopMVC.Data.Models;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Sync
{
    public interface ICharacter
    {
        Character GetByID(int? id);
        IEnumerable<Character> GetAll();
        IEnumerable<Character> GetAllWithGames();
        Character GetByIDOrRandom(int? id);

        Character Add(Character character);
        Character Update(Character character);
        void Delete(int? id);
    }
}
