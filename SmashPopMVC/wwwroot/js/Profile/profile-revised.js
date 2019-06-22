function attachFormButtonEvent(buttons, url, dataType, successCallBack, errorCallBack, type) {
    if (successCallBack == null) {
        successCallBack = function (res) { }
    }
    if (errorCallBack == null) {
        errorCallBack = function (ts) { swal("Error!", ts, "warning"); };
    }
    if (type == null) {
        type = "POST";
    }
    if (dataType == null) {
        dataType = "json";
    }

    buttons.off('click');
    buttons.on('click', function (e) {
        e.preventDefault();
        const form = $(this).parent();
        sendAjaxRequest(url, form, dataType, successCallBack, errorCallBack, type);
        return false;
    });
}

function successRemoveButtonCallBack(res, form) {
    swal("Success!", res.responseText, "success");
    form.children('button').remove();
}

function loadNewComment(res, requestForm) {
    const commentParent = requestForm.parent().parent().parent();
    const newCommentForm = createObjectFromHtml(res, 'form');
    commentParent.prepend(newCommentForm);
    const callBack = function (inputArea) { submitNewComment(inputArea.parent()) };
    const input = newCommentForm.children('.content-input');
    attachCommentSubmitEvent(input, callBack);
    input.focus();
}

function attachImageEvent(images) {
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


$(document).ready(function () {
    const profileImages = $('#ProfileImages .updatable');
    attachImageEvent(profileImages);

    const updateUserButton = $('#UpdateUser');
    attachFormButtonEvent(updateUserButton, '/ApplicationUser/Update', successRemoveButtonCallBack);

    const addFriendButton = $('#AddFriend');
    attachFormButtonEvent(addFriendButton, '/ApplicationUser/AddFriend', successRemoveButtonCallBack);

    const acceptFriendButtons = $('.accept-friend');
    attachFormButtonEvent(acceptFriendButtons, '/ApplicationUser/AcceptFriend');

    const newCommentButton = $('#NewComment');
    attachFormButtonEvent(newCommentButton, '/Comment/New', 'html', loadNewComment);

    buildProfileComments();
});
