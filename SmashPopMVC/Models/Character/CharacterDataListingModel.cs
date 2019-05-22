using System.Collections.Generic;

namespace SmashPopMVC.Models.Character
{
    public class CharacterDataListingModel
    {
        public IEnumerable<CharacterDataModel> CharacterList { get; set; }
    }
}
