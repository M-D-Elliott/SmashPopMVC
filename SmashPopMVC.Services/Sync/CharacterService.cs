using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Service.Sync
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


        public Character Add(Character character)
        {
            var characterReturnValue = _context.Characters.Add(character);
            _context.SaveChanges();
            return characterReturnValue.Entity;
        }

        public IEnumerable<Character> GetAll()
        {
            return _context.Characters
                .ToList();
        }

        public IEnumerable<Character> GetAllWithGames()
        {
            return _context.Characters
                .Include(c => c.Origin)
                .Include(c => c.SmashOrigin)
                .ToList();
        }

        public Character GetByID(int? id)
        {
            return _context.Characters
                .Single(c => c.ID == id);
        }

        public Character GetByIDOrRandom(int? id)
        {
            if (id == null)
            {
                int toSkip = _random.Next(0, _context.Characters.ToList().Count - 1);
                return _context.Characters
                    .Skip(toSkip)
                    .Take(1)
                    .First();
            }
            else
            {
                return _context.Characters
                    .FirstOrDefault(c => c.ID == id);
            }
        }

        public void Delete(int? id)
        {
            var character = _context.Characters.FirstOrDefaultAsync(c => c.ID == id);
            if(character == null)
            {
                throw new InvalidOperationException();
            }
            _context.Remove(character);
            _context.SaveChanges();
        }

        public Character Update(Character character)
        {
            throw new NotImplementedException();
        }
    }
}
