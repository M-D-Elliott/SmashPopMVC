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

        public Comment Get(int commentID)
        {
            return _context.Comments
                .Where(c => c.ID == commentID)
                .FirstOrDefault();
        }

        public IEnumerable<Comment> GetUserProfileCommentsWithReplies(string userID)
        {
            throw new NotImplementedException();
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
