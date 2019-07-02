using System.Collections.Generic;

namespace SmashPopMVC.Models.Comment
{
    public class CommentDataModel : CommentModel
    {
        public int ID { get; set; }
        public string PosterID { get; set; }
        public string PosterName { get; set; }
        public IEnumerable<CommentDataModel> Replies { get; set; }
        public bool Deleted { get; set; }

        public string CurrentUserID { get; set; }
        public NewCommentModel NewCommentModel { get; set; }
        public int MaxDepth { get; set; }
    }
}
