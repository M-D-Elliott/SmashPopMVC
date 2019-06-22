using SmashPopMVC.Data.Models;
using SmashPopMVC.Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Services
{
    public interface ICommentPackager
    {

        CommentListingModel BuildCommentListing(string profileUserID, string currentUserID);
        MoreCommentsModel BuildMoreComments(string profileUserID, string currentUserID, int? lastCommentID);
        IEnumerable<CommentDataModel> BuildCommentDataList(IEnumerable<Comment> comments, string currentUserID, int take = 20);
        NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID);
        //CommentListingModel BuildCommentListing(IEnumerable<Comment> comments, string profileUserID, string currentUserID);
        //MoreCommentsModel BuildMoreComments(IEnumerable<Comment> comments, string profileUserID, string currentUserID);
        //IEnumerable<CommentDataModel> BuildCommentDataList(IEnumerable<Comment> comments, string currentUserID, int take = 20);
        //NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID);
    }
}
