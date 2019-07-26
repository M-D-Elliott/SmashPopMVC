using SmashPopMVC.Data;
using System.Collections.Generic;
using SmashPopMVC.Data.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SmashPopMVC.Data.Async;
using System;

namespace SmashPopMVC.Service.Async
{
    public class CommentService : IComment
    {
        public readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> Add(Comment comment)
        {
            var commentReturnValue = _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return commentReturnValue.Entity;
        }

        public async Task<Comment> Get(int? commentID, bool replies = false)
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

            return await comment
                .Include(c => c.Poster)
                .FirstOrDefaultAsync(c => c.ID == commentID);
        }

        public async Task<Comment> Update(int id, Comment comment)
        {
            if (!_context.Comments.Any(c => c.ID == id))
            {
                throw new InvalidOperationException();
            }
            var commentReturnValue = _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return commentReturnValue.Entity;
        }

        public async Task<Comment> Delete(int id)
        {
            var comment = await Get(id);
            if (comment != null)
            {
                comment.Content = "Comment has been removed by user.";
                comment.Deleted = true;
                return await Update(id, comment);
            }
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetChildComments(int parentID)
        {
            return await _context.Comments
                    .Where(c => c.ReplyToID == parentID)
                    .Include(r => r.Poster)
                    .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetByPostee(string posteeID, int? lastCommentID, int take)
        {
            var comments = _context.Comments
                .Where(c => c.PosteeID == posteeID)
                .Where(c => c.ReplyToID == null);


            if (lastCommentID != null)
            {
                comments = comments
                    .Where(c => c.ID < lastCommentID);
            }
            return await comments
                .OrderByDescending(c => c.Created)
                .Include(c => c.Poster)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> SearchComments(string userID, string searchQuery)
        {
            return await _context.Comments
                .ToListAsync();
        }
    }
}
