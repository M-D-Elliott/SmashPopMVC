using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Services.Async
{
    public interface ICommentPackager
    {
        Task<CommentListingModel> BuildCommentListing(string profileUserID, string currentUserID);
        Task<MoreCommentsModel> BuildMoreComments(string profileUserID, string currentUserID, int? lastCommentID);
        Task<IEnumerable<CommentDataModel>> BuildCommentDataList(IEnumerable<Comment> comments, string currentUserID, int take);
        NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID);
        CommentDataModel BuildCommentEditModel(Comment comment);
    }
}
