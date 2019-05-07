using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmashPopMVC.Data.Models
{
    public class Game
    {
        public int ID { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string SubTitle { get; set; }
        public DateTime ReleaseDate { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string Discriminator { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
    }
}
