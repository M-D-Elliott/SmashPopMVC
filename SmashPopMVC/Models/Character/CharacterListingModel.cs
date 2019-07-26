using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Character
{
    public class CharacterListingModel
    {
        public IEnumerable<CharacterDataModel> CharacterList { get; set; } 
    }
}
