using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Service.Sync
{
    public class GameService : IGame
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Game> GetAll(string discriminator = null)
        {
            if(!String.IsNullOrEmpty(discriminator))
            {
                return _context.Games.Where(g => g.Discriminator == discriminator).Include(g => g.Characters);
            }
            return _context.Games.Include(g => g.Characters);
        }

        public IEnumerable<Game> GetOriginGames()
        {
            return GetAll(discriminator: "OriginGames");
        }

        public Game GetByID(int id)
        {
            return _context.Games.Where(g => g.ID == id)
                .Include(g => g.Characters)
                .FirstOrDefault();
        }

        public IEnumerable<Game> GetSmashGames()
        {
            return GetAll(discriminator: "SmashGames");
        }

        public void Add(Data.Models.Game game)
        {
            _context.Add(game);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var game = _context.Games.Find(id);
            if (game.Title != "Super Smash Bros")
            {
                _context.Remove(game);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Cannot delete Super Smash Brothers games.");
            }
        }
    }
}
