using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class Tally
    {
        public Tally()
        {
            Votes = new List<Vote>();
        }

        public int ID { get; set; }
        public DateTime Created { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public virtual IEnumerable<Vote> Votes { get; set; }
    }
}
