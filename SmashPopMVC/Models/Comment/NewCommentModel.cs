using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class NewCommentModel : CommentModel
    {
        public string PosteeID { get; set; }
        public string PosterID { get; set; }
        public int? ReplyToID { get; set; }
    }
}
