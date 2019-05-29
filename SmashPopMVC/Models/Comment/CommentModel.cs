using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class CommentModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Created { get; set; }
        public string PosterName { get; set; }

    }
}
