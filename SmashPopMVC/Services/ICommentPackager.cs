﻿using SmashPopMVC.Data.Models;
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
        IEnumerable<CommentDataModel> BuildCommentDataList(IEnumerable<Comment> comments, string currentUserID, int take, int maxDepth = 5);
        NewCommentModel BuildNewCommentModel(string posteeID, string posterID, int? replyToID, int maxDepth = 5);
        CommentDataModel BuildCommentEditModel(Comment comment, int maxDepth = 5);
    }
}
