using SmashPopMVC.Models.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Vote
{
    public class VoteDataModel
    {
        public string Created { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public CharacterDataModel MostDifficult { get; set; }
        public CharacterDataModel LeastDifficult { get; set; }
        public CharacterDataModel FlavorOfTheMonth { get; set; }
        public CharacterDataModel MostPowerful { get; set; }
    }
}
