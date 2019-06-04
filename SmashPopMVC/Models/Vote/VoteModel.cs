using SmashPopMVC.Models.Character;
using SmashPopMVC.Models.Tally;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Vote
{
    public class VoteModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public CharacterDataModel MostDifficult { get; set; }
        public CharacterDataModel LeastDifficult { get; set; }
        public CharacterDataModel FlavorOfTheMonth { get; set; }
        public CharacterDataModel MostPowerful { get; set; }

        public TallyDataModel TallyData { get; set; }
    }
}
