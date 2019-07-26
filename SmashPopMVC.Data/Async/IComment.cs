using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmashPopMVC.Data.Async
{
    public interface IComment
    {
        Task<Comment> Get(int? commentID, bool replies = false);
        Task<Comment> Add(Comment comment);
        Task<Comment> Update(int id, Comment comment);
        Task<Comment> Delete(int id);
        Task<IEnumerable<Comment>> SearchComments(
            string userID,
            string searchQuery);
        Task<IEnumerable<Comment>> GetChildComments(int parentID);
        Task<IEnumerable<Comment>> GetByPostee(string posteeID, int? lastCommentID, int take);
    }
}
