using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Character
{
    public class CharacterDataModel
    {
        public int ID { get; set; }
        public string SmashID { get; set; }
        public string Name { get; set; }
        public string Tier { get; set; }
        public string ImageName { get; set; }
        public GameDataModel Origin { get; set; }
        public GameDataModel SmashOrigin { get; set; }
    }
}
