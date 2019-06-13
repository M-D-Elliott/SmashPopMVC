using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
{
    public interface IComment
    {
        Comment Get(int? commentID);
        void Add(Comment comment);
        void Update(Comment comment);

        IEnumerable<Comment> GetUserProfileCommentsWithReplies(string userID);
        IEnumerable<Comment> SearchComments(
            string userID,
            string searchQuery);
    }
}
