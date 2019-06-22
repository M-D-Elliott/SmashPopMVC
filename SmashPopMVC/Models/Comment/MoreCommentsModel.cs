using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class MoreCommentsModel
    {
        public IEnumerable<CommentDataModel> Comments { get; set; }
        public LoadCommentsModel LoadCommentsModel { get; set; }
    }
}
