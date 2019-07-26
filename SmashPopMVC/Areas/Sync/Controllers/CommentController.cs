using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Services.Sync;
using SmashPopMVC.Areas.Shared.Attributes.Validators;

namespace SmashPopMVC.Areas.Sync.Controllers
{
    [Area("Sync")]
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

        //[ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 3, Name = "NewComment", Message = "request new comments")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult New(NewCommentModel model)
        {
            if(model == null)
            {
                return BadRequest("How did you get here?");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }
            return PartialView(model);
        }

        //[ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 3, Name = "PostComments", Message = "post comments")]
        [RequireHttps, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(NewCommentModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }

            var replyTo = model.ReplyToID == null ? null : _commentService.Get(model.ReplyToID);
            var depth = replyTo == null ? 0 : replyTo.Depth + 1;
            if (depth >= model.MaxDepth)
            {
                return BadRequest("Max comment depth exceeded.");
            }

            var comment = new Comment
            {
                Postee = _applicationUserService.Get(model.PosteeID),
                Poster = _applicationUserService.Get(model.PosterID),
                ReplyTo = replyTo,
                Content = model.Content,
                Created = DateTime.Now,
                Replies = new List<Comment>(),
                Depth = depth,
            };
            try
            {
                comment = _commentService.Add(comment);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
            if(comment == null)
            {
                return BadRequest("How did you get here?");
            }
            var dataModel = _commentPackager.BuildCommentEditModel(comment);
            return PartialView("Edit", dataModel);
;        }

        [ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 30, Name = "ViewMoreComments", Message = "request comment history")]
        [RequireHttps, HttpGet, ValidateAntiForgeryToken]
        public IActionResult More(LoadCommentsModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }

            var dataModel = _commentPackager.BuildMoreComments(model.ProfileUserID, model.CurrentUserID, model.LastCommentID);
            if (dataModel.Comments != null && dataModel.Comments.Any())
            {
                return PartialView(dataModel);
            }
            return Content("");
        }

        //[ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 4, Name = "RequestEditComment", Message = "request comment edits.")]
        public IActionResult Edit(CommentDataModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }
            return PartialView(model);
        }

        //[ThrottleByCount(TimeUnit = TimeUnit.Minute, Count = 3, Name = "EditComment", Message = "edit comments")]
        [HttpPut, ValidateAntiForgeryToken]
        public IActionResult Update(CommentDataModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }

            var comment = _commentService.Get(model.ID);
            if (comment == null)
            {
                return NotFound("Comment does not exist.");
            }
            comment.Content = model.Content;
            try
            {
                comment = _commentService.Update(comment.ID, comment);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            var dataModel = _commentPackager.BuildCommentEditModel(comment);
            return PartialView("Edit", dataModel);

        }

        [ThrottleByCount(TimeUnit = TimeUnit.Hour, Count = 50, Name = "DeleteComments", Message = "delete comments")]
        [RequireHttps, HttpPut, ValidateAntiForgeryToken]
        public IActionResult Delete(CommentDataModel model)
        {
            if (model == null)
            {
                return BadRequest("How did you get here?");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("I know what you're doing! ;)");
            }

            var comment = _commentService.Delete(model.ID);
            if(comment != null)
            {
                var dataModel = _commentPackager.BuildCommentEditModel(comment);
                return PartialView("Edit", dataModel);
            }
            else
            {
                return NotFound("Comment does not exist.");
            }

        }
    }
}