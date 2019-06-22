using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class LoadCommentsModel
    {
        public string CurrentUserID { get; set; }
        public string ProfileUserID { get; set; }
        public int? LastCommentID { get; set; }
        public int Take { get; set; }
    }
}
