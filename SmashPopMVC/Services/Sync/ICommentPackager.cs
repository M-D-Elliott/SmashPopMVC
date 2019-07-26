using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmashPopMVC.Services.Sync
{
    public interface ICommentPackager
    {
        CommentListingModel BuildCommentListing(string profileUserID, string currentUserID);
        MoreCommentsModel BuildMoreComments(string profileUserID, string currentUserID, int? lastCommentID);
        IEnumerable<CommentDataModel> BuildCommentDataList(IEnumerable<Comment> comments, string currentUserID, int take);
        NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID);
        CommentDataModel BuildCommentEditModel(Comment comment);
    }
}
