//|||| General functions |||||

//Takes raw html, perhaps an ajax response, and
// ad jQ identifying string, such as a class then 
// turns it into a real JQ object, ready to attach 
// to any html element.
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

// Attachs any callback which takes the resulting 
// form when instantiated to a button, or any JQ object.
function attachButtonEvents(buttons, callBack) {
    buttons.off('click');
    buttons.on('click', function(e) {
        e.preventDefault();
        const form = $(this).parent();
        callBack(form);
        return false;
    });
}

// passes an anonymous function containing the sendAjaxRequest function into
// the attachButtonEvents function as it's callback. It also injects an
// interceding validation callBack, such as confirmationCallBack if this
// argument is supplied.
function attachAjaxButtonEvents(buttons, url, dataType, successCallBack, errorCallBack, type, validationCallBack) {
    if (validationCallBack == undefined || validationCallBack == null) {
        validationCallBack = function (form, callBack) { callBack(form) }
    }
    const ajaxRequest = function (form) { sendAjaxRequest(url, form, dataType, successCallBack, errorCallBack, type) };

    let validation = ajaxRequest;
    if (validationCallBack != undefined && validationCallBack != null) {
        validation = function (form) {
            validationCallBack(form, ajaxRequest)
        }
    }
    attachButtonEvents(buttons, validation);
}

// attaches the specific submit events related to image interaction
// with a selection modal.
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

// Attaches events related to submitting user-input fields, such as
// text boxes, activating on either leaving the input field or
// pressing the enter key.
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

// attaches an event that activates the callBack when the user scrolls
// the supplied object. A height object that is a child of that object that
// will be full height of all it's children are needed. the trigger percent
// determines the portion scrolled before the callBack is called.
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

// Attaches the scroll event above to the comments, using the default scroll value.
function attachCommentScrollEvents(profileComments) {
    const commentsBody = profileComments.children('#comments-body');
    const lastCommentID = commentsBody.children(".LoadCommentsForm").children(".LastCommentID");
    if (lastCommentID.val()) {
        const requestCommentsCallBack = function (scrollObject) { requestMoreComments(scrollObject) };
        attachScrollEvents(profileComments, commentsBody, requestCommentsCallBack);
    }
}

// |||||| Data Interface builders. ||||||

// builds character selecting events and user-update events
// using attach functions found above.
function buildProfileDataEvents(profileData) {
    const addFriendButton = profileData.find('#AddFriend');
    attachAjaxButtonEvents(addFriendButton, '/Friend/Add', 'json', successRemoveButtonCallBack);

    const requestPartnershipButton = profileData.find('#RequestPartnership');
    attachAjaxButtonEvents(requestPartnershipButton, '/Friend/RequestPartnership', 'json', successRemoveButtonCallBack, null, 'PUT');

    const message = "Your partnership will be cleared.";
    const confirmTerminatePartner = function (form, callBack) {
        basicConfirmationCallBack(message, form, callBack);
    };
    const cancelPartnershipButton = profileData.find('#CancelPartnership');
    attachAjaxButtonEvents(cancelPartnershipButton, '/Friend/CancelPartnership', 'json', successRemoveFormParentCallBack, null, 'PUT', confirmTerminatePartner);

    const profileImages = profileData.find('#UserProfileImages .updatable');
    attachImageEvents(profileImages);

    const updateUserButton = profileData.find('#UpdateUser');
    attachAjaxButtonEvents(updateUserButton, '/ApplicationUser/Update', 'json', successRemoveButtonCallBack, null, 'PUT');
}

// Attaches ajax button events to Friend and Partnership related buttons.
function buildFriendsEvents(profileFriends) {
    const acceptFriendButtons = profileFriends.find('.accept-friend');
    attachAjaxButtonEvents(acceptFriendButtons, '/Friend/Accept', 'json', acceptFriendCallBack, null, 'PUT');
}

//Attaches ajax button events to comment related buttons.
function buildCommentsEvents(parent) {
    const newCommentButton = parent.find('#NewComment');
    const newCommentCallBack = function (res, form) { loadNewCommentForm(res, form.siblings('#comments-body')) };
    attachAjaxButtonEvents(newCommentButton, '/Comment/New', 'html', newCommentCallBack, null, 'POST');

    const replyButtons = parent.find('.NewCommentForm .comment-reply');
    const replyButtonsCallBack = function (res, form) { loadNewCommentForm(res, form.parent().siblings('.replies')) };
    attachAjaxButtonEvents(replyButtons, '/Comment/New', 'html', replyButtonsCallBack, null, 'POST');

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
    attachAjaxButtonEvents(deleteButtons, '/Comment/Delete', "html", deleteButtonsCallBack, null, 'PUT', confirmDelete);
}

//|||| Simple Success CallBacks ||||||

// Removes the button that triggered the callback as a success function.
function standardErrorCallBack(ts, callBack) {
    if (callBack == undefined || callBack == null) {
        callBack = function (ts, form) { };
    }
    try {
        errorCallBack = function (ts, form) { standardErrorAlert(ts); callBack(ts, form); };
    }
    catch (err) {
        errorCallBack = function (ts, form) { console.log(ts.error); callBack(ts, form); };
    }
    return errorCallBack;
}

function successRemoveButtonCallBack(res, form) {
    if (res.success) {
        swal("Success!", res.responseText, "success");
        form.children('button').hide();
    } else {
        standardErrorAlert(res);
    }
}

function successRemoveFormParentCallBack(res, form) {
    if (res.success) {
        swal("Success!", res.responseText, "success");
        form.parent().hide();
    } else {
        standardErrorAlert(res);
    }
}

// Changes the user images as a success callback.
function changeUserImage(modal, mainOrAlt, modalClass) {
    if (changeCharacterCard(modal, mainOrAlt, modalClass)) {
        $('#UpdateUser').show();
    }
}

// Places the newly approved friend in the accepted friend box.
function acceptFriendCallBack(res, form) {
    const card = form.parent();
    form.remove();
    const approvedFriends = $("#ApprovedFriends");
    $("#noFriendsMessage").remove();
    approvedFriends.children(".friends-holder").append(card);
}

// |||||| Simple validation Callbacks ||||

// Uses sweet alerts to confirm the user is sure of their action.
// used for deletion and partership removal.
function basicConfirmationCallBack(text, form, callBack) {
    swal({
        title: "Are you sure?",
        text: text,
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then(function(confirm)  {
        if (confirm) {
            callBack(form);
        }
    });
}


//|||| Comment CallBacks ||||||

// builds comments that have newly arrived from the server
// such as newly placed comments, those that have been
// edited, or deleted. Also attaches events.
function buildComment(res) {
    const comment = createObjectFromHtml(res, '.comment');
    const links = comment.children('.comment-links');
    buildCommentsEvents(links);
    return comment;
}

// attaches the newly created comment from the buildComment
// to a parent and removes the form used to create it.
function buildNewComment(res, commentParent) {
    commentParent.prepend(buildComment(res));
}

// attaches more comments when they arrive from server,
// including a fresh more comments form, which contains
// the id of the most recently loaded comment. Also attaches
// scroll events.
function buildAndPlaceMoreComments(res, form) {
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

// uses the build comment function above to place an edited comment
// after receiving it from the server.
function buildAndPlaceRevisedComment(res, originalComment) {
    const revisedComment = buildComment(res);
    const replies = originalComment.children('.replies').children();
    revisedComment.insertBefore(originalComment);
    revisedComment.children('.replies').append(replies);
    originalComment.remove();
}

// sends an ajax request and attaches a function which builds and
// places the new comments as well as their load more comments form to
// bottom of the comment list.
function requestMoreComments(profileComments) {
    const form = profileComments.children("#comments-body").children('.LoadCommentsForm');
    const url = "/Comment/More";
    const successCallBack = function (res, form) { buildAndPlaceMoreComments(res, form) };
    const errorCallBack = function (ts) { console.log(ts) };
    sendAjaxRequest(url, form, "html", successCallBack, errorCallBack, null);
}

//|||| Comment request callbacks ||||

// attaches reply events to a newly generated comment.
function loadNewCommentForm(res, commentParent) {
    const newCommentForm = createObjectFromHtml(res, 'form');
    commentParent.prepend(newCommentForm);
    const input = newCommentForm.children('.content-input');
    const url = "/Comment/Add";
    const callBack = function (inputArea) {
        const successCallBack = function (res) {
            buildNewComment(res, commentParent);
        };
        const form = inputArea.parent();
        errorCallBack = function (ts, form) { standardErrorCallBack(ts)(ts, form); };
        sendAjaxRequest(url, form, "html", successCallBack, errorCallBack);
        form.remove();
    };
    attachInputEvents(input, callBack);
    input.focus();
}

// hides the comment's content field, attaches the comment's edit
// comment form to the main comment area and makes it visible.
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

// attaches the build and place revised comment success callback as
// well as the comment restore error callback to ajax request to
// update the comment.
function submitCommentEdit(inputArea) {
    let form = inputArea.parent();
    const originalComment = form.parent();
    if (inputArea.val() != originalComment.children('.comment-content').text()) {
        const successCallBack = function (res, form) { buildAndPlaceRevisedComment(res, originalComment) };
        const errorCallBack = function (ts) { commentRestore(originalComment); console.log('error:', ts) };
        const url = '/Comment/Update';
        sendAjaxRequest(url, form, 'html', successCallBack, errorCallBack, 'PUT');
    } else {
        commentRestore(originalComment, form);
    }
    form.remove()
}

//||||Post-Loading Actions ||||||

// primarily used to attach button events through builders after page loads,
// replacing any "return false" submit on the form buttons.
$(document).ready(function () {

    // Profile Data events.
    const profileData = $("#ProfileData");
    buildProfileDataEvents(profileData);

    // Friends events.
    const profileFriends = $("#ProfileFriends");
    buildFriendsEvents(profileFriends);

    // Comments events.
    const profileComments = $('#ProfileComments');
    buildCommentsEvents(profileComments);
    attachCommentScrollEvents(profileComments);
});
