using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class CommentDataModel : CommentModel
    {
        public SmashPopMVC.Data.Models.Comment Comment { get; set; }
        public IEnumerable<CommentDataModel> Replies { get; set; }
    }
}
