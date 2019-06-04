using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Vote
{
    public class NewVoteModel : VoteModel
    {
        public DateTime Created { get; set; }
        public string MonthOfConcern { get; set; }
    }
}
