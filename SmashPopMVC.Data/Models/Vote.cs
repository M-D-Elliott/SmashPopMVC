using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class Vote
    {
        public int ID { get; set; }

        public string VoterID { get; set; }
        public virtual ApplicationUser Voter { get; set; }
        public DateTime Created { get; set; }

        public int TallyID { get; set; }
        public virtual Tally Tally { get; set; }

        public int MostDifficultID { get; set; }
        public virtual Character MostDifficult { get; set; }

        public int? LeastDifficultID { get; set; }
        public virtual Character LeastDifficult { get; set; }

        public int? FlavorOfTheMonthID { get; set; }
        public virtual Character FlavorOfTheMonth { get; set; }

        public int? MostPowerfulID { get; set; }
        public virtual Character MostPowerful { get; set; }

    }
}
