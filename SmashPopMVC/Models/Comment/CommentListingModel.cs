using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class CommentListingModel
    {
        public IEnumerable<CommentDataModel> Comments { get; set; }
        public NewCommentModel NewCommentModel { get; set; }
    }
}
