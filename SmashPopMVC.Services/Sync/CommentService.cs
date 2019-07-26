using SmashPopMVC.Data;
using System.Collections.Generic;
using SmashPopMVC.Data.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data.Sync;
using System;

namespace SmashPopMVC.Service.Sync
{
    public class CommentService : IComment
    {
        public readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Comment Add(Comment comment)
        {
            var commentReturnValue = _context.Comments.Add(comment);
            _context.SaveChanges();
            return commentReturnValue.Entity;
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
                .FirstOrDefault(c => c.ID == commentID);
        }

        public Comment Update(int id, Comment comment)
        {
            if (!_context.Comments.Any(c => c.ID == id))
            {
                throw new InvalidOperationException();
            }
            var commentReturnValue = _context.Comments.Update(comment);
            _context.SaveChanges();
            return commentReturnValue.Entity;
        }

        public Comment Delete(int id)
        {
            var comment = Get(id);
            if (comment != null)
            {
                comment.Content = "Comment has been removed by user.";
                comment.Deleted = true;
                return Update(id, comment);
            }
            return comment;
        }

        public IEnumerable<Comment> GetChildComments(int parentID)
        {
            return _context.Comments
                    .Where(c => c.ReplyToID == parentID)
                    .Include(r => r.Poster)
                    .ToList();
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
                .Take(take)
                .ToList();
        }

        public IEnumerable<Comment> SearchComments(string userID, string searchQuery)
        {
            return _context.Comments
                .ToList();
        }
    }
}
