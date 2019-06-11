using System.Collections.Generic;

namespace SmashPopMVC.Models.Comment
{
    public class CommentDataModel : CommentModel
    {
        public int ID { get; set; }
        public string PosterID { get; set; }
        public IEnumerable<CommentDataModel> Replies { get; set; }
        public NewCommentModel NewCommentModel { get; set; }
    }
}
