using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Game
{
    public class GameDataModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Release { get; set; }
        public int Year { get; set; }
    }
}
