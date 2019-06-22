using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Models.Comment
{
    public class CommentModel
    {
        public string Content { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
