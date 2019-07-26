using SmashPopMVC.Data.Models;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Sync
{
    public interface IComment
    {
        Comment Get(int? commentID, bool replies = false);
        Comment Add(Comment comment);
        Comment Update(int id, Comment comment);
        Comment Delete(int id);
        IEnumerable<Comment> SearchComments(
            string userID,
            string searchQuery);
        IEnumerable<Comment> GetChildComments(int parentID);
        IEnumerable<Comment> GetByPostee(string posteeID, int? lastCommentID, int take);
    }
}
