using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;

namespace SmashPopMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IComment _commentService;
        private readonly IApplicationUser _applicationUserService;

        public CommentController(IComment commentService, IApplicationUser applicationUserService)
        {
            _commentService = commentService;
            _applicationUserService = applicationUserService;
        }

        [HttpPost]
        public IActionResult New(NewCommentModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AddCommentModel model)
        {
            var comment = new Comment
            {
                Postee = _applicationUserService.GetUser(model.PosteeID),
                Poster = _applicationUserService.GetUser(model.PosterID),
                ReplyTo = _commentService.Get(model.ReplyToID),
                Title = model.Title,
                Content = model.Content,
                Created = DateTime.Now,
                Replies = new List<Comment>(),
            };
            if (ModelState.IsValid)
            {
                _commentService.Add(comment);
                var dataModel = BuildCommentEditModel(comment, comment.Poster.Id);
                return PartialView("Edit", dataModel);
            }
            else
            {
                return NotFound();
            }
        }
        
        public IActionResult Edit(CommentDataModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Update(CommentDataModel model)
        {
            var comment = _commentService.Get(model.ID);
            comment.Content = model.Content;
            comment.Title = model.Title;
            _commentService.Update(comment);
        }

        private CommentDataModel BuildCommentEditModel(Comment comment, string currentUserID)
        {
            var newCommentModel = BuildNewCommentModel(comment.PosteeID, currentUserID, comment.ID);
            return new CommentDataModel
            {
                ID = comment.ID,
                Title = comment.Title,
                Content = comment.Content,
                Replies = null,
                PosterName = comment.Poster.ShortName,
                Created = comment.Created.ToString(),
                NewCommentModel = newCommentModel,
            };
        }

        private NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID)
        {
            return new NewCommentModel
            {
                PosteeID = posteeID,
                PosterID = posterID,
                ReplyToID = replyToID == null ? null : replyToID,
                Content = "",
                Title = "",
            };
        }
    }
}