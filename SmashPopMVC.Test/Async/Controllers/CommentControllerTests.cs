using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Areas.Async.Controllers;
using SmashPopMVC.Data.Async;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Extensions;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Services.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.UnitTest.Async.Controllers
{
    [TestFixture]
    public class CommentControllerTests
    {
        private int _maxDepth = 5;
        public ApplicationUser UserFactory() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-rgrg",
            UserName = "Yo@gmail.com",
            ShortName = "Yo"
        };

        public ApplicationUser UserFactory2() => new ApplicationUser
        {
            Id = "4444-hhhh-jjjj-kkkk-2grg",
            UserName = "Lol@gmail.com",
            ShortName = "Lol"
        };

        public Comment CommentFactory() => new Comment
        {
            ID = 1,
            Postee = UserFactory(),
            Poster = UserFactory2(),
            Created = DateTime.Now,
            Content = "Hello, my name is Mr spings, and I enjoy it.",
            ReplyToID = null,
            ReplyTo = null,
            Depth = 0,
            Deleted = false,
            Replies = new List<Comment>
            {
            },
        };

        public Comment CommentFactoryReply() => new Comment
        {
            ID = 2,
            Postee = UserFactory(),
            Poster = UserFactory(),
            Created = DateTime.Now,
            Content = "Hello, my name is Mr spings, and I enjoy it.",
            ReplyToID = 1,
            ReplyTo = CommentFactory(),
            Depth = 1,
            Deleted = false,
            Replies = new List<Comment>
            {
            },
        };

        public Comment CommentFactoryReplyReply() => new Comment
        {
            ID = 3,
            Postee = UserFactory(),
            Poster = UserFactory2(),
            Created = DateTime.Now,
            Content = "Hello, my name is Mr spings, and I enjoy it.",
            ReplyToID = 2,
            ReplyTo = CommentFactoryReply(),
            Depth = 2,
            Deleted = false,
            Replies = new List<Comment>
            {
            },
        };

        public Comment CommentFactoryReply2() => new Comment
        {
            ID = 4,
            Postee = UserFactory(),
            Poster = UserFactory(),
            Created = DateTime.Now,
            Content = "Hello, my name is Mr spings, and I enjoy it.",
            ReplyToID = 1,
            ReplyTo = CommentFactory(),
            Depth = 1,
            Deleted = false,
            Replies = new List<Comment>
            {
            },
        };

        public Comment CommentFactoryNoReplies() => new Comment
        {
            ID = 1,
            Postee = UserFactory(),
            Poster = UserFactory2(),
            Created = DateTime.Now,
            Content = "Hello, my name is Mr spings, and I enjoy it.",
            ReplyToID = null,
            ReplyTo = null,
            Depth = 0,
            Deleted = false,
            Replies = new List<Comment>
            {
            },
        };

        [Test]
        public void New_ValidModel_ReturnsPartialViewResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var currentUser = UserFactory2();
            var comment = CommentFactory();
            //var commentReply = CommentFactoryReply();
            //var commentReply2 = CommentFactoryReply2();
            //var commentReplyReply = CommentFactoryReplyReply();
            //commentReply.Replies.ToList().Add(commentReplyReply);
            //comment.Replies.ToList().AddRange(new List<Comment> {
            //    commentReply,
            //    commentReply2,
            //});

            var model = new NewCommentModel
            {
            };

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackagerMock.Object);

            var result = controller.New(model);

            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is NewCommentModel);
            var newCommentModel = viewModel as NewCommentModel;
            Assert.IsTrue(viewModel == model);
        }

        [Test]
        public void New_NullModel_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackagerMock.Object);

            var result = controller.New(null);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void New_InvalidModel_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackagerMock.Object);
            controller.ModelState.AddModelError("BadModel", "This model contains invalid data.");

            var model = new NewCommentModel { };
            var result = controller.New(model);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task Add_ValidModel_ReturnsPartialViewResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();
            comment.Depth = 3;

            var model = new NewCommentModel
            {
                ReplyToID = comment.ID,
                Content = comment.Content,
                PosteeID = comment.Postee.Id,
                PosterID = comment.Poster.Id,
                MaxDepth = _maxDepth,
            };

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(comment));
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(Task.FromResult(comment.Postee))
                                      .Returns(Task.FromResult(comment.Poster));
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Returns(Task.FromResult(comment));

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Add(It.IsAny<Comment>()));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
        }

        [Test]
        public async Task Add_NullModel_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Add(null);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task Add_ModelError_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);
            controller.ModelState.AddModelError("BadModel", "This model contains invalid data.");
            var result = await controller.Add(null);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task Add_DepthLimitExceeded_ReturnsBadRequestResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();
            comment.Depth = 4;

            var model = new NewCommentModel
            {
                ReplyToID = comment.ID,
                MaxDepth = _maxDepth,
            };
            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(comment));
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(Task.FromResult(comment.Postee))
                                      .Returns(Task.FromResult(comment.Poster));
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Returns(Task.FromResult(comment));
            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task Add_ServiceAddReturnsError_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();

            var model = new NewCommentModel
            {
                ReplyToID = comment.ID,
                MaxDepth = _maxDepth,
            };
            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult((Comment)null));
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(Task.FromResult(comment.Postee))
                                      .Returns(Task.FromResult(comment.Poster));
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Throws(new Exception());
            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Add(It.IsAny<Comment>()));
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task Add_ServiceAddReturnsNull_ReturnsNotFoundObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();

            var model = new NewCommentModel
            {
                ReplyToID = comment.ID,
                MaxDepth = _maxDepth,
            };
            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult((Comment)null));
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(Task.FromResult(comment.Postee))
                                      .Returns(Task.FromResult(comment.Poster));
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Returns(Task.FromResult((Comment)null));
            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Add(It.IsAny<Comment>()));
            Assert.That(result is NotFoundObjectResult);
        }

        [Test]
        public void Edit_ValidModel_ReturnsPartialViewResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var currentUser = UserFactory2();
            var comment = CommentFactory();

            var model = new CommentDataModel
            {
            };

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackagerMock.Object);

            var result = controller.Edit(model);

            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
            var retModel = viewModel as CommentDataModel;
            Assert.IsTrue(viewModel == model);
        }

        [Test]
        public void Edit_NullModel_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackagerMock.Object);

            var result = controller.Edit(null);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void Edit_InvalidModel_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackagerMock = new Mock<ICommentPackager>();

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackagerMock.Object);
            controller.ModelState.AddModelError("BadModel", "This model contains invalid data.");

            var model = new CommentDataModel { };
            var result = controller.Edit(model);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task More_ValidModel_ReturnsPartialViewResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();
            var commentReply = CommentFactoryReply();
            var commentReply2 = CommentFactoryReply2();
            var commentNoReplies = CommentFactoryNoReplies();
            comment.Replies = new List<Comment>{
                commentReply,
                commentReply2,
            };

            var comments = new List<Comment>
            {
                comment,
                commentNoReplies,
            };

            var model = new LoadCommentsModel
            {
                CurrentUserID = comment.Poster.Id,
                ProfileUserID = comment.Postee.Id,
                LastCommentID = 5,
                Take = 20,
            };

            commentServiceMock.Setup(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(comments as IEnumerable<Comment>));

            commentServiceMock.SetupSequence(cs => cs.GetChildComments(It.IsAny<int>()))
                              .Returns(Task.FromResult(comment.Replies as IEnumerable<Comment>))
                              .Returns(Task.FromResult((IEnumerable<Comment>)null))
                              .Returns(Task.FromResult((IEnumerable<Comment>)null))
                              .Returns(Task.FromResult(commentNoReplies.Replies as IEnumerable<Comment>));

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.More(model);

            commentServiceMock.Verify(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
            commentServiceMock.Verify(cs => cs.GetChildComments(It.IsAny<int>()), Times.Exactly(4));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is MoreCommentsModel);
        }

        [Test]
        public async Task More_GetByPosteeReturnsEmptyCommentsList_ReturnsEmptyContentResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var comment = CommentFactory();

            var model = new LoadCommentsModel
            {
                CurrentUserID = comment.Poster.Id,
                ProfileUserID = comment.Postee.Id,
                LastCommentID = 5,
                Take = 20,
            };

            commentServiceMock.Setup(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new List<Comment>() as IEnumerable<Comment>));


            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.More(model);

            commentServiceMock.Setup(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
            Assert.True(result is ContentResult);
        }

        [Test]
        public async Task Update_ValidModel_ReturnsPartialViewResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);
            
            var comment = CommentFactory();

            var model = new CommentDataModel
            {
                ID = comment.ID,
                Content = "New Content",
            };

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(comment));

            commentServiceMock.Setup(cs => cs.Update(It.IsAny<int>(), It.IsAny<Comment>())).Returns(Task.FromResult(comment));

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Update(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
        }

        [Test]
        public async Task Update_CommentDoesNotExist_ReturnsJsonSuccessFalse()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);
            
            var comment = CommentFactory();

            var model = new CommentDataModel
            {
                ID = comment.ID,
                Content = "New Content",
            };

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult((Comment)null));

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Update(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is JsonResult);
            JsonResult jsonResult = result as JsonResult;
            var dict = jsonResult.AsRouteValueDictionary();
            Assert.That(dict["success"], Is.False);
        }

        [Test]
        public async Task Update_CommentServiceUpdateThrowsError_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);
            
            var comment = CommentFactory();

            var model = new CommentDataModel
            {
                ID = comment.ID,
                Content = "New Content",
            };

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(comment));

            commentServiceMock.Setup(cs => cs.Update(It.IsAny<int>(), It.IsAny<Comment>())).Throws(new InvalidOperationException { });

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Update(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Update(It.IsAny<int>(), It.IsAny<Comment>()));
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public async Task Delete_ValidModel_ReturnsPartialViewResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();

            var model = new CommentDataModel
            {
                ID = comment.ID,
            };

            commentServiceMock.Setup(cs => cs.Delete(It.IsAny<int>())).Returns(Task.FromResult(comment));

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Delete(model);

            commentServiceMock.Verify(cs => cs.Delete(It.IsAny<int>()));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
        }

        [Test]
        public async Task Delete_CommentDoesNotExist_ReturnsNotFoundObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var currentUser = UserFactory2();
            var comment = CommentFactory();

            var model = new CommentDataModel
            {
                ID = comment.ID,
            };

            commentServiceMock.Setup(cs => cs.Delete(It.IsAny<int>())).Returns(Task.FromResult((Comment)null));

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = await controller.Delete(model);

            commentServiceMock.Verify(cs => cs.Delete(It.IsAny<int>()));
            Assert.That(result is NotFoundObjectResult);
        }
    }
}
