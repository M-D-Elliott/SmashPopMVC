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
                Title = model.Title,
                Content = model.Content,
                Created = DateTime.Now,
                Replies = new List<Comment>(),
            };
            if (ModelState.IsValid)
            {
                _commentService.Add(comment);
                var dataModel = new CommentDataModel
                {
                    ID = comment.ID,
                    Title = comment.Title,
                    Content = comment.Content,
                    PosterName = comment.Poster.ShortName,
                    Created = comment.Created.ToString(),

                };
                return PartialView("Edit", dataModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
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
    }
}