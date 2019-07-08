﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Controllers.Attributes.Validators;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Services;

namespace SmashPopMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IComment _commentService;
        private readonly IApplicationUser _applicationUserService;
        private readonly ICommentPackager _commentPackager;

        public CommentController(IComment commentService, IApplicationUser applicationUserService, ICommentPackager commentPackager)
        {
            _commentService = commentService;
            _applicationUserService = applicationUserService;
            _commentPackager = commentPackager;
        }
        
        [ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 3, Name = "NewComment", Message = "request new comments")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult New(NewCommentModel model)
        {
            return PartialView(model);
        }

        public IActionResult Edit(CommentDataModel model)
        {
            return PartialView(model);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 30, Name = "ViewMoreComments", Message = "request comment history")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult More(LoadCommentsModel model)
        {
            var dataModel = _commentPackager.BuildMoreComments(model.ProfileUserID, model.CurrentUserID, model.LastCommentID);
            if (dataModel.Comments != null && dataModel.Comments.Any())
            {
                return PartialView(dataModel);
            }
            return Content("");
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 3, Name ="PostComments", Message ="post comments")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(AddCommentModel model)
        {
            if (ModelState.IsValid)
            {
                var replyTo = _commentService.Get(model.ReplyToID);
                var comment = new Comment
                {
                    Postee = _applicationUserService.Get(model.PosteeID),
                    Poster = _applicationUserService.Get(model.PosterID),
                    ReplyTo = replyTo,
                    Content = model.Content,
                    Created = DateTime.Now,
                    Replies = new List<Comment>(),
                    Depth = replyTo == null ? 1 : replyTo.Depth + 1,
                };
                if (comment.Depth <= model.MaxDepth)
                {
                    _commentService.Add(comment);
                    var dataModel = _commentPackager.BuildCommentEditModel(comment);
                    return PartialView("Edit", dataModel);
                }
                else
                {
                    return Json(new { success = false, responseText = "Max comment depth exceeded." });
                }

            }
            else
            {
                return NotFound();
            }
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 3, Name = "EditComments", Message = "edit comments")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(CommentDataModel model)
        {
            var comment = _commentService.Get(model.ID);
            comment.Content = model.Content;
            _commentService.Update(comment);
            var dataModel = _commentPackager.BuildCommentEditModel(comment);
            return PartialView("Edit", dataModel);
        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 50, Name = "DeleteComments", Message = "delete comments")]
        [RequireHttps, HttpDelete, ValidateAntiForgeryToken]
        public IActionResult Delete(CommentDataModel model)
        {
            var comment = _commentService.Delete(model.ID);
            if(comment != null)
            {
                var dataModel = _commentPackager.BuildCommentEditModel(comment);
                return PartialView("Edit", dataModel);
            }
            else
            {
                return Json(new { success = false, responseText = "The comment could not be found." });
            }

        }
    }
}