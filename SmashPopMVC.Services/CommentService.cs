using System;
using SmashPopMVC.Data;
using System.Collections.Generic;
using System.Text;
using SmashPopMVC.Data.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SmashPopMVC.Service
{
    public class CommentService : IComment
    {
        public readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public Comment Delete(int ID)
        {
            var comment = Get(ID);
            if(comment != null)
            {
                comment.Content = "Comment has been removed by user.";
                comment.Deleted = true;
                Update(comment);
            }
            return comment;
        }

        public Comment Get(int? commentID, bool replies = false)
        {
            if(commentID == null)
            {
                return null;
            }
            var comment = _context.Comments
                .Where(c => c.ID == commentID);

            if (replies)
            {
                comment = comment
                    .Include(c => c.Replies);
            }

            return comment
                .Include(c => c.Poster)
                .FirstOrDefault();
        }

        public IEnumerable<Comment> GetChildComments(int parentID)
        {
            return _context.Comments
                    .Where(c => c.ReplyToID == parentID)
                    .Include(r => r.Poster);
        }

        public IEnumerable<Comment> GetByPostee(string posteeID, int? lastCommentID, int take)
        {
            var comments = _context.Comments
                .Where(c => c.PosteeID == posteeID)
                .Where(c => c.ReplyToID == null);


            if (lastCommentID != null)
            {
                comments = comments
                    .Where(c => c.ID < lastCommentID);
            }
            return comments
                .OrderByDescending(c => c.Created)
                .Include(c => c.Poster)
                .Take(take);
        }

        public IEnumerable<Comment> SearchComments(string userID, string searchQuery)
        {
            throw new NotImplementedException();
        }

        public void Update(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
