using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Character
{
    public class CharacterListingModel
    {
        public int ID { get; set; }
        public string SmashID { get; set; }
        public string Name { get; set; }
        public string Tier { get; set; }
        public string OriginTitle { get; set; }
        public string DebutDate { get; set; }
        public string SmashDebutTitle { get; set; }
        public string SmashDebutDate { get; set; }
    }
}
