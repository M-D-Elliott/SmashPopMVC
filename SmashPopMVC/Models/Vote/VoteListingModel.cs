using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Vote
{
    public class VoteListingModel
    {
        public IEnumerable<VoteDataModel> Results { get; set; }
        public EditVoteModel NewVoteModel { get; set; }
        public bool CurrentVote { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
