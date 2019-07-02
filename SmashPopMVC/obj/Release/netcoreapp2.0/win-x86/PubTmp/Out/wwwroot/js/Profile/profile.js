//|||| General functions |||||

function createObjectFromHtml(html, jQIdentifier) {
    const temp_object = $('#SmashPopTemp');
    temp_object.html(html);
    var domObject = temp_object.children(jQIdentifier).clone();
    if (!domObject.length) {
        console.log('No object called ' + jQIdentifier + ' found.');
    }
    temp_object.html('');
    return domObject;
}

//|||| Event attachers ||||||

function attachButtonEvents(buttons, callBack) {
    buttons.off('click');
    buttons.on('click', function(e) {
        e.preventDefault();
        const form = $(this).parent();
        callBack(form);
        return false;
    });
}

function attachAjaxButtonEvents(buttons, url, dataType, successCallBack, errorCallBack, type, validationCallBack) {
    if (validationCallBack == undefined || validationCallBack == null) {
        validationCallBack = function (form, callBack) { callBack(form) }
    }
    const ajaxRequest = function (form) { sendAjaxRequest(url, form, dataType, successCallBack, errorCallBack, type) };
    const validation = function (form) {
        validationCallBack(form, ajaxRequest)
    }
    attachButtonEvents(buttons, validation);
}

function attachImageEvents(images) {
    images.off('click');
    images.on('click', function () {
        const select = this;
        const type = $(this).attr('id');
        const addClass = 'main-alt-select';
        const modal = $('#modal-container .modal-content');
        const callBack = function () {
            changeUserImage(modal, type, addClass)
        };
        loadCharacterModal(modal, select, 1, callBack, addClass);
    });
}

function attachInputEvents(inputAreas, callBack) {
    inputAreas.blur(function (e) {
        callBack($(this));
        return false;
    });
    inputAreas.on("keydown", function (e) {
        if (e.key == "Enter") {
            e.preventDefault();
            callBack($(this));
            return false;
        }
    });
}

function attachScrollEvents(scrollObject, heightObject, callBack, triggerPercent) {
    if (triggerPercent == undefined || triggerPercent == null) {
        triggerPercent = 95;
    }
    if (heightObject == undefined || heightObject == null) {
        heightObject = scrollObject;
    }
    scrollObject.on("scroll", function (e) {
        const percent = 100 * $(this).scrollTop() / (heightObject.height() - $(this).height());
        if (percent > triggerPercent) {
            callBack($(this));
            scrollObject.off("scroll");
        }
    });
}

function attachCommentScrollEvents(profileComments) {
    const commentsBody = profileComments.children('#comments-body');
    const lastCommentID = commentsBody.children(".LoadCommentsForm").children(".LastCommentID");
    if (lastCommentID.val()) {
        const requestCommentsCallBack = function (scrollObject) { requestMoreComments(scrollObject) };
        attachScrollEvents(profileComments, commentsBody, requestCommentsCallBack);
    }
}

function buildCommentsEvents(parent) {
    const replyButtons = parent.find('.NewCommentForm .comment-reply');
    const replyButtonsCallBack = function (res, requestForm) { loadNewCommentForm(res, requestForm.parent().siblings('.replies')) };
    attachAjaxButtonEvents(replyButtons, '/Comment/New', 'html', replyButtonsCallBack);

    const editButtons = parent.find('.EditCommentForm .comment-edit');
    const editFormSubmitCallBack = function (inputArea) { submitCommentEdit(inputArea) };
    const editButtonsCallBack = function (form) { revealEditCommentForm(form, editFormSubmitCallBack) };
    attachButtonEvents(editButtons, editButtonsCallBack);

    const deleteButtons = parent.find('.DeleteCommentForm .comment-delete');
    const deleteButtonsCallBack = function (res, form) {
        buildAndPlaceRevisedComment(res, form.parent().parent());
        swal("Success!", "Comment deleted.", "success");
    };
    const message = "You will not be able to recover deleted comments.";
    const confirmDelete = function (form, callBack) {
        basicConfirmationCallBack(message, form, callBack);
    };
    attachAjaxButtonEvents(deleteButtons, '/Comment/Delete', "html", deleteButtonsCallBack, null, "DELETE", confirmDelete);
}

//|||| Simple Success CallBacks ||||||

function successRemoveButtonCallBack(res, form) {
    swal("Success!", res.responseText, "success");
    form.children('button').hide();
}

function changeUserImage(modal, mainOrAlt, modalClass) {
    if (changeCharacterCard(modal, mainOrAlt, modalClass)) {
        $('#UpdateUser').show();
    }
}

function acceptFriendCallBack(res, form) {
    const card = form.parent();
    form.remove();
    const approvedFriends = $("#ApprovedFriends");
    approvedFriends.children("#noFriendsMessage").remove();
    approvedFriends.children(".friends-holder").append(card);
}

// |||||| Simple validation CallBack ||||
function basicConfirmationCallBack(text, form, callBack) {
    swal({
        title: "Are you sure?",
        text: text,
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then((confirm) => {
        if (confirm) {
            callBack(form);
        }
    });
}


//|||| Comment CallBacks ||||||

function buildComment(res) {
    const comment = createObjectFromHtml(res, '.comment');
    const links = comment.children('.comment-links');
    buildCommentsEvents(links);
    return comment;
}

function buildNewComment(res, form, commentParent) {
    commentParent.prepend(buildComment(res));
    form.remove();
}

function commentRestore(originalComment) {
    originalComment.children('.comment-content').show();
}

function buildAndPlaceRevisedComment(res, originalComment) {
    const revisedComment = buildComment(res);
    const replies = originalComment.children('.replies').children();
    revisedComment.insertBefore(originalComment);
    revisedComment.children('.replies').append(replies);
    originalComment.remove();
}

function requestMoreComments(profileComments) {
    const form = profileComments.children("#comments-body").children('.LoadCommentsForm');
    const url = "/Comment/More";
    const successCallBack = function (res, form) { buildAndPlaceComments(res, form) };
    const errorCallBack = function (ts) { console.log(ts) };
    sendAjaxRequest(url, form, "html", successCallBack, errorCallBack, null);
}

function buildAndPlaceComments(res, form) {
    if (res.length) {
        const commentsBody = form.parent();
        const profileComments = commentsBody.parent();
        form.remove();
        const newComments = createObjectFromHtml(res, '.comment, form');
        buildCommentsEvents(newComments);
        commentsBody.append(newComments);
        attachCommentScrollEvents(profileComments);
    } else {
        console.error("No more comments.");
    }

}

//|||| Comment request callbacks ||||

function loadNewCommentForm(res, commentParent) {
    const newCommentForm = createObjectFromHtml(res, 'form');
    commentParent.prepend(newCommentForm);
    const input = newCommentForm.children('.content-input');
    const url = "/Comment/Add";
    const callBack = function (inputArea) {
        const successCallBack = function (res, form) { buildNewComment(res, form, commentParent); };
        sendAjaxRequest(url, inputArea.parent(), "html", successCallBack)
    };
    attachInputEvents(input, callBack);
    input.focus();
}

function revealEditCommentForm(form, submitCallBack) {
    const comment = form.parent().parent();
    const commentContent = comment.children('.comment-content');
    commentContent.hide();

    form = form.clone();
    comment.prepend(form);
    const formInput = form.children('.content-input');
    const editButton = form.children('.comment-edit');
    editButton.hide();
    formInput.show();
    attachInputEvents(formInput, submitCallBack);
    formInput.focus();
}

function submitCommentEdit(inputArea) {
    let form = inputArea.parent();
    const originalComment = form.parent();
    if (inputArea.val() != originalComment.children('.comment-content').text()) {
        const successCallBack = function (res, form) { buildAndPlaceRevisedComment(res, originalComment) };
        const errorCallBack = function (ts) { commentRestore(originalComment); console.log('error:', ts) };
        const url = '/Comment/Update';
        sendAjaxRequest(url, form, 'html', successCallBack, errorCallBack)
    } else {
        commentRestore(originalComment, form);
    }
    form.remove()
}

//||||Post-Loading Actions ||||||

$(document).ready(function () {
    const profileImages = $('#ProfileImages .updatable');
    attachImageEvents(profileImages);

    const updateUserButton = $('#UpdateUser');
    attachAjaxButtonEvents(updateUserButton, '/ApplicationUser/Update', 'json', successRemoveButtonCallBack);

    const addFriendButton = $('#AddFriend');
    attachAjaxButtonEvents(addFriendButton, '/ApplicationUser/AddFriend', 'json', successRemoveButtonCallBack);

    const acceptFriendButtons = $('.accept-friend');
    attachAjaxButtonEvents(acceptFriendButtons, '/ApplicationUser/AcceptFriend', 'json', acceptFriendCallBack);

    const newCommentButton = $('#NewComment');
    const newCommentCallBack = function (res, form) { loadNewCommentForm(res, form.siblings('#comments-body')) };
    attachAjaxButtonEvents(newCommentButton, '/Comment/New', 'html', newCommentCallBack);

    const profileComments = $('#ProfileComments');
    const commentsBody = profileComments.children('#comments-body');
    buildCommentsEvents(commentsBody);
    attachCommentScrollEvents(profileComments);
});
