using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SmashPopMVC.Services.Sync
{
    public class CommentPackager : ICommentPackager
    {
        private readonly IComment _commentService;
        private readonly int _maxDepth;

        public CommentPackager(IComment commentService)
        {
            _commentService = commentService;
            _maxDepth = 5;
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
                lastCommentID = commentDataList.LastOrDefault().ID;
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
            var commentList = comments
                .Select(c =>
                {
                    var depth_exceeded = c.Depth >= _maxDepth - 1;
                    var replies_body = depth_exceeded ? null : _commentService.GetChildComments(c.ID);
                    bool replies_null = replies_body == null || !replies_body.Any();
                    return new CommentDataModel
                    {
                        ID = c.ID,
                        Content = c.Content,
                        Date = c.Created.ToString("yyyy-MM-dd"),
                        Time = c.Created.ToString("HH:mm:ss"),
                        PosterName = c.Poster.UserName.Substring(0, c.Poster.UserName.IndexOf('@')),
                        PosterID = c.Poster.Id,
                        Deleted = c.Deleted,
                        CurrentUserID = currentUserID,
                        Replies = (replies_null) ? null : BuildCommentDataList(replies_body, currentUserID, 1000),
                        NewCommentModel = (depth_exceeded) ? null : BuildNewCommentModel(c.PosteeID, currentUserID, c.ID),
                        MaxDepth = _maxDepth,
                    };
                });
            return commentList;
        }

        public NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID)
        {
            return new NewCommentModel
            {
                Content = "",
                PosteeID = posteeID,
                PosterID = posterID,
                ReplyToID = replyToID == null ? null : replyToID,
                MaxDepth = _maxDepth,
            };
        }

        public CommentDataModel BuildCommentEditModel(Comment comment)
        {
            var depth_exceeded = comment.Depth >= _maxDepth - 1;
            var newCommentModel = depth_exceeded ? null : BuildNewCommentModel(comment.PosteeID, comment.Poster.Id, comment.ID);
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
                CurrentUserID = comment.Poster.Id,
                NewCommentModel = newCommentModel,
                MaxDepth = _maxDepth
            };
        }
    }
}
