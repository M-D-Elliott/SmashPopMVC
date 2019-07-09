using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class CharVote
    {
        public int TallyID { get; set; }
        public virtual Tally Tally { get; set; }

        public int CharacterID { get; set; }
        public Character Character { get; set; }
    }
}
