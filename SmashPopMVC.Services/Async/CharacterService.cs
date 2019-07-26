using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Service.Async
{
    public class CharacterService : ICharacter
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random;

        public CharacterService(ApplicationDbContext context)
        {
            _context = context;
            _random = new Random();
        }


        public async Task<Character> Add(Character character)
        {
            var characterReturnValue = _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            return characterReturnValue.Entity;
        }

        public async Task<IEnumerable<Character>> GetAll()
        {
            return await _context.Characters
                .ToListAsync();
        }

        public async Task<IEnumerable<Character>> GetAllWithGames()
        {
            return await _context.Characters
                .Include(c => c.Origin)
                .Include(c => c.SmashOrigin)
                .ToListAsync();
        }

        public async Task<Character> GetByID(int? id)
        {
            return await _context.Characters
                .SingleAsync(c => c.ID == id);
        }

        public async Task<Character> GetByIDOrRandom(int? id)
        {
            if (id == null)
            {
                int toSkip = _random.Next(0, _context.Characters.ToList().Count - 1);
                return await _context.Characters
                    .Skip(toSkip)
                    .Take(1)
                    .FirstAsync();
            }
            else
            {
                return await _context.Characters
                    .FirstOrDefaultAsync(c => c.ID == id);
            }
        }

        public async Task Delete(int? id)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.ID == id);
            if(character == null)
            {
                throw new InvalidOperationException();
            }
            _context.Remove(character);
            await _context.SaveChangesAsync();
        }

        public Task<Character> Update(Character character)
        {
            throw new NotImplementedException();
        }
    }
}
