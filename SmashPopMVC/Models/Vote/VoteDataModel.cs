using SmashPopMVC.Models.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Vote
{
    public class VoteDataModel : VoteModel
    {
        public string Created { get; set; }
        public bool Editable { get; set; }
    }
}
