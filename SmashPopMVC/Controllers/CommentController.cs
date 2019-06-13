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
                Content = model.Content,
                Created = DateTime.Now,
                Replies = new List<Comment>(),
            };
            if (ModelState.IsValid)
            {
                _commentService.Add(comment);
                var dataModel = BuildCommentEditModel(comment);
                return PartialView("Edit", dataModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CommentDataModel model)
        {
            var comment = _commentService.Get(model.ID);
            comment.Content = model.Content;
            _commentService.Update(comment);
            var dataModel = BuildCommentEditModel(comment);
            return PartialView("Edit", dataModel);
        }

        public IActionResult Edit(CommentDataModel model)
        {
            return PartialView(model);
        }

        private CommentDataModel BuildCommentEditModel(Comment comment)
        {
            var newCommentModel = BuildNewCommentModel(comment.PosteeID, comment.Poster.Id, comment.ID);
            return new CommentDataModel
            {
                ID = comment.ID,
                Content = comment.Content,
                Replies = null,
                PosterName = comment.Poster.ShortName,
                PosterID = comment.Poster.Id,
                Date = comment.Created.ToString("yyyy-MM-dd"),
                Time = comment.Created.ToString("HH:mm:ss"),
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
            };
        }
    }
}