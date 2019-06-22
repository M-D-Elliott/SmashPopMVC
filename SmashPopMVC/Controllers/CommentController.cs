using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Services;

namespace SmashPopMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IApplicationUser _applicationUserService;
        private readonly ICommentPackager _commentPackager;

        public CommentController(IApplicationUser applicationUserService, ICommentPackager commentPackager)
        {
            _applicationUserService = applicationUserService;
            _commentPackager = commentPackager;
        }

        [HttpPost]
        public IActionResult New(NewCommentModel model)
        {
            return PartialView(model);
        }

        public IActionResult Edit(CommentDataModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult More(LoadCommentsModel model)
        {
            var dataModel = _commentPackager.BuildMoreComments(model.ProfileUserID, model.CurrentUserID, model.LastCommentID);
            if (dataModel.Comments != null && dataModel.Comments.Any())
            {
                return PartialView(dataModel);
            }
            return Content("");
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
                _commentPackager._commentService.Add(comment);
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

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(CommentDataModel model)
        {
            var comment = _commentService.Delete(model.ID);
            if(comment != null)
            {
                var dataModel = BuildCommentEditModel(comment);
                return PartialView("Edit", dataModel);
            }
            else
            {
                return Json(new { success = false, responseText = "The comment could not be found." });
            }

        }

        private CommentDataModel BuildCommentEditModel(Comment comment)
        {
            var newCommentModel = _commentPackager.BuildNewCommentModel(comment.PosteeID, comment.Poster.Id, comment.ID);
            return new CommentDataModel
            {
                ID = comment.ID,
                Content = comment.Content,
                Replies = null,
                Deleted = comment.Deleted,
                PosterName = comment.Poster.ShortName,
                PosterID = comment.Poster.Id,
                Date = comment.Created.ToString("yyyy-MM-dd"),
                Time = comment.Created.ToString("HH:mm:ss"),
                NewCommentModel = newCommentModel,
            };
        }
    }
}