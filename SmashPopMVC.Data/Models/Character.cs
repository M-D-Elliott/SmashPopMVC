using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class Character
    {
        public int ID { get; set; }

        [Column(TypeName = "varchar(4)")]
        public string SmashID { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(4)")]
        public string Tier { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Popularity { get; set; }

        [ForeignKey("Origin")]
        public int? OriginID { get; set; }
        [ForeignKey("SmashOrigin")]
        public int SmashOriginID { get; set; }
        [InverseProperty("Characters")]
        public virtual OriginGame Origin { get; set; }
        [InverseProperty("Characters")]
        public virtual SmashGame SmashOrigin { get; set; }

        [Column(TypeName = "varchar(34)")]
        public string ImageName { get; set; }
        public int SmashPopMains { get; set; }
        public int SmashPopAlts { get; set; }

        //Voting data
        //public virtual IEnumerable<Vote> Votes { get; set; }

        public virtual IEnumerable<Vote> MostDifficultVotes { get; set; }
        public virtual IEnumerable<Vote> LeastDifficultVotes { get; set; }
        public virtual IEnumerable<Vote> FlavorOfTheMonthVotes { get; set; }
        public virtual IEnumerable<Vote> MostPowerfulVotes { get; set; }
    }
}
