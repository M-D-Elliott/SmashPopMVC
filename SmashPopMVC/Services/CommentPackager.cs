using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Services;
using System.Collections.Generic;
using System.Linq;

namespace SmashPopMVC.Services
{
    public class CommentPackager : ICommentPackager
    {
        private readonly IComment _commentService;

        public CommentPackager(IComment commentService)
        {
            _commentService = commentService;
        }

        public CommentListingModel BuildCommentListing(string profileUserID, string currentUserID)
        {
            return new CommentListingModel
            {
                NewCommentModel = BuildNewCommentModel(profileUserID, currentUserID, null),
                MoreCommentsModel = BuildMoreComments(profileUserID, currentUserID, null),
            };
        }

        public MoreCommentsModel BuildMoreComments(string profileUserID, string currentUserID, int? lastCommentID)
        {
            int take = 15;
            var comments = _commentService.GetByPostee(profileUserID, lastCommentID, take);
            var commentDataList = BuildCommentDataList(comments, currentUserID, take);
            lastCommentID = null;
            if (commentDataList.Any() && commentDataList.LastOrDefault() != commentDataList.FirstOrDefault())
            {
                lastCommentID =  commentDataList.LastOrDefault().ID;
            }

            var loadCommentsModel = new LoadCommentsModel
            {
                CurrentUserID = currentUserID,
                ProfileUserID = profileUserID,
                Take = take,
                LastCommentID = lastCommentID,
            };
            var model = new MoreCommentsModel
            {
                Comments = commentDataList,
                LoadCommentsModel = loadCommentsModel,
            };
            return model;
        }

        public IEnumerable<CommentDataModel> BuildCommentDataList(IEnumerable<Comment> comments, string currentUserID, int take)
        {
            return comments
                .Select(c => new CommentDataModel
                {
                    ID = c.ID,
                    Content = c.Content,
                    Date = c.Created.ToString("yyyy-MM-dd"),
                    Time = c.Created.ToString("HH:mm:ss"),
                    PosterName = c.Poster.UserName.Substring(0, c.Poster.UserName.IndexOf('@')),
                    PosterID = c.Poster.Id,
                    Deleted = c.Deleted,
                    Replies = BuildCommentDataList(_commentService.GetChildComments(c.ID), currentUserID, take: 1000),
                    NewCommentModel = BuildNewCommentModel(c.PosteeID, currentUserID, c.ID),
                });
        }

        public NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID)
        {
            return new NewCommentModel
            {
                Content = "",
                PosteeID = posteeID,
                PosterID = posterID,
                ReplyToID = replyToID == null ? null : replyToID,
            };
        }

        public CommentDataModel BuildCommentEditModel(Comment comment)
        {
            var newCommentModel = BuildNewCommentModel(comment.PosteeID, comment.Poster.Id, comment.ID);
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
