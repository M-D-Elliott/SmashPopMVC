using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashPopMVC.Data
{
    public interface IComment
    {
        Comment Get(int? commentID, bool replies = false);
        void Add(Comment comment);
        void Update(Comment comment);
        Comment Delete(int ID);
        IEnumerable<Comment> SearchComments(
            string userID,
            string searchQuery);
        IEnumerable<Comment> GetChildComments(int parentID);
        IEnumerable<Comment> GetByPostee(string posteeID, int? lastCommentID, int take);
        //Comment Get(int? commentID, bool replies = false);
        //void Add(Comment comment);
        //void Update(Comment comment);
        //Comment Delete(int ID);
        //IEnumerable<Comment> SearchComments(
        //    string userID,
        //    string searchQuery);
        //IEnumerable<Comment> GetMoreComments(string profileUserID, int? lastCommentID, int take);
    }
}
