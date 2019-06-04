using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Vote
{
    public class VoteListingModel
    {
        public IEnumerable<VoteDataModel> Results { get; set; }
        public NewVoteModel NewVoteModel { get; set; }
    }
}
