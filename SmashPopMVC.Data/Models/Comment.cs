using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class Comment
    {
        public Comment()
        {
            Deleted = false;
        }

        public int ID { get; set; }
        [Column(TypeName = "nvarchar(15)")]
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }

        [ForeignKey("Postee")]
        public string PosteeID { get; set; }
        public virtual ApplicationUser Postee { get; set; }
        public virtual ApplicationUser Poster { get; set; }

        [ForeignKey("ReplyTo")]
        public int? ReplyToID { get; set; }
        public virtual Comment ReplyTo { get; set; }

        public virtual IEnumerable<Comment> Replies { get; set; }
        public int Depth { get; set; }

        public bool Deleted { get; set; }
    }
}
