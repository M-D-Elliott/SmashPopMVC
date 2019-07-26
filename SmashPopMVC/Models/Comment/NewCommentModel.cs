using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class NewCommentModel
    {
        public string PosteeID { get; set; }
        public string PosterID { get; set; }
        public int? ReplyToID { get; set; }
        public int Depth { get; set; }
        public int MaxDepth { get; set; }
        public string Content { get; set; }
    }
}
