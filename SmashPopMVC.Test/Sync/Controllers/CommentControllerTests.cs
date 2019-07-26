using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Data.Models;
using SmashPopMVC.Extensions;
using SmashPopMVC.Models.Comment;
using SmashPopMVC.Services.Sync;
using System;
using System.Collections.Generic;
using SmashPopMVC.Areas.Sync.Controllers;

namespace SmashPopMVC.UnitTest.Sync.Controllers
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
            Content = "Yo mr springs",
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
            Content = "Did you really have to greet him?",
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
            Content = "I bet you do.",
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
            ID = 5,
            Postee = UserFactory(),
            Poster = UserFactory2(),
            Created = DateTime.Now,
            Content = "All the posts above this one are wrong.",
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
        public void Add_ValidModel_ReturnsPartialViewResult()
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

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(comment);
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(comment.Postee)
                                      .Returns(comment.Poster);
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Returns(comment);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Add(It.IsAny<Comment>()));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
        }

        [Test]
        public void Add_NullModel_ReturnsJsonSuccessFalse()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Add(null);

            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void Add_ModelError_ReturnsBadRequestObjectResult()
        {
            var commentServiceMock = new Mock<IComment>();
            var applicationUserServiceMock = new Mock<IApplicationUser>();
            var commentPackager = new CommentPackager(commentServiceMock.Object);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);
            controller.ModelState.AddModelError("BadModel", "This model contains invalid data.");
            var result = controller.Add(null);
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void Add_DepthLimitExceeded_ReturnsBadRequestObjectResult()
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
            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(comment);
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(comment.Postee)
                                      .Returns(comment.Poster);
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Returns(comment);
            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void Add_ServiceAddReturnsError_ReturnsBadRequestObjectResult()
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
            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((Comment)null);
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(comment.Postee)
                                      .Returns(comment.Poster);
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Throws(new Exception());
            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Add(It.IsAny<Comment>()));
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void Add_ServiceAddReturnsNull_ReturnsBadRequestObjectResult()
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
            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((Comment)null);
            applicationUserServiceMock.SetupSequence(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()))
                                      .Returns(comment.Postee)
                                      .Returns(comment.Poster);
            commentServiceMock.Setup(cs => cs.Add(It.IsAny<Comment>())).Returns((Comment)null);
            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Add(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            applicationUserServiceMock.Verify(aus => aus.Get(It.IsAny<string>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Add(It.IsAny<Comment>()));
            Assert.That(result is BadRequestObjectResult);
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
        public void More_ValidModel_ReturnsPartialViewResult()
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

            commentServiceMock.Setup(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(comments as IEnumerable<Comment>);

            commentServiceMock.SetupSequence(cs => cs.GetChildComments(It.IsAny<int>()))
                              .Returns(comment.Replies as IEnumerable<Comment>)
                              .Returns(commentReply.Replies as IEnumerable<Comment>)
                              .Returns(commentReply2.Replies as IEnumerable<Comment>)
                              .Returns(commentNoReplies.Replies as IEnumerable<Comment>)
                              .Returns((IEnumerable<Comment>)null);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.More(model);

            commentServiceMock.Verify(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
            commentServiceMock.Verify(cs => cs.GetChildComments(It.IsAny<int>()), Times.Exactly(5));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is MoreCommentsModel);
        }

        [Test]
        public void More_GetByPosteeReturnsEmptyCommentsList_ReturnsEmptyContentResult()
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

            commentServiceMock.Setup(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Comment>() as IEnumerable<Comment>);


            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.More(model);

            commentServiceMock.Setup(cs => cs.GetByPostee(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
            Assert.True(result is ContentResult);
        }

        [Test]
        public void Update_ValidModel_ReturnsPartialViewResult()
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

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(comment);

            commentServiceMock.Setup(cs => cs.Update(It.IsAny<int>(), It.IsAny<Comment>())).Returns(comment);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Update(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
        }

        [Test]
        public void Update_CommentDoesNotExist_ReturnsNotFoundObjectResult()
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

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns((Comment)null);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Update(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            Assert.That(result is NotFoundObjectResult);
        }

        [Test]
        public void Update_CommentServiceUpdateThrowsError_BadRequestObjectResult()
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

            commentServiceMock.Setup(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(comment);

            commentServiceMock.Setup(cs => cs.Update(It.IsAny<int>(), It.IsAny<Comment>())).Throws(new InvalidOperationException { });

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Update(model);

            commentServiceMock.Verify(cs => cs.Get(It.IsAny<int>(), It.IsAny<bool>()));
            commentServiceMock.Verify(cs => cs.Update(It.IsAny<int>(), It.IsAny<Comment>()));
            Assert.That(result is BadRequestObjectResult);
        }

        [Test]
        public void Delete_ValidModel_ReturnsPartialViewResult()
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

            commentServiceMock.Setup(cs => cs.Delete(It.IsAny<int>())).Returns(comment);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Delete(model);

            commentServiceMock.Verify(cs => cs.Delete(It.IsAny<int>()));
            Assert.True(result is PartialViewResult);
            var viewModel = (result as PartialViewResult).Model;
            Assert.True(viewModel is CommentDataModel);
        }

        [Test]
        public void Delete_CommentDoesNotExist_ReturnsNotFoundObjectResult()
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

            commentServiceMock.Setup(cs => cs.Delete(It.IsAny<int>())).Returns((Comment)null);

            var controller = new CommentController(commentServiceMock.Object,
                                                   applicationUserServiceMock.Object,
                                                   commentPackager);

            var result = controller.Delete(model);

            commentServiceMock.Verify(cs => cs.Delete(It.IsAny<int>()));
            Assert.That(result is NotFoundObjectResult);
        }
    }
}
