using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Service
{
    public class CharacterService : ICharacter
    {
        private readonly ApplicationDbContext _context;

        public CharacterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Character> GetAll()
        {
            return _context.Characters;
        }

        public IEnumerable<Character> GetAllWithGames()
        {
            return _context.Characters
                .Include(c => c.Origin)
                .Include(c => c.SmashOrigin);
        }

        public Character GetByID(int? id)
        {
            if(id == null)
            {
                return null;
            }
            //return _context.Characters.Where(c => c.ID == id)
            //    .FirstOrDefault();
            return _context.Characters.FirstOrDefault(c => c.ID == id);
        }

        public async Task Add(Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var character = _context.Characters.Find(id);
            _context.Remove(character);
            await _context.SaveChangesAsync();
        }
    }
}
