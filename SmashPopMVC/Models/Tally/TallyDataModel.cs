using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Tally
{
    public class TallyDataModel
    {
        public int ID { get; set; }
        public DateTime Created { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}
