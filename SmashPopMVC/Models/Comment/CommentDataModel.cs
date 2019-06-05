using System.Collections.Generic;

namespace SmashPopMVC.Models.Comment
{
    public class CommentDataModel : CommentModel
    {
        public int ID { get; set; }
        public IEnumerable<CommentDataModel> Replies { get; set; }
    }
}
