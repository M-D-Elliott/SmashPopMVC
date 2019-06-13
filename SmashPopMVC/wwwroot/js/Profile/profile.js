function changeUserImage(modal, mainOrAlt, modalClass) {
    if (changeCharacterCard(modal, mainOrAlt, modalClass)) {
        $('#UpdateUser').show();
    }
}

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

function attachCommentSubmit(inputAreas, callBack) {
    inputAreas.blur(function (e) {
        callBack($(this));
        activeForm = false;
        return false;
    });

    //forms.on("keydown", ":input:not(textarea)", function (e) {
    //    return event.key != "Enter";
    //});
    inputAreas.on("keydown", function (e) {
        if (e.key == "Enter") {
            e.preventDefault();
            callBack($(this));
            activeForm = false;
            return false;
        }
    });
}

function requestReplyForm(form) {
    const commentParent = form.parent().siblings('.replies');
    requestNewCommentForm(form, commentParent);
}

function buildComment(res) {
    const comment = createObjectFromHtml(res, '.comment');
    const links = comment.children('.comment-links');

    const replyButtons = links.find('.NewCommentForm .comment-reply');
    attachCommentReplyEvents(replyButtons);

    const editButtons = links.find('.EditCommentForm .edit-button');
    const inputAreas = links.find('.EditCommentForm .content-input');
    attachCommentEditEvents(editButtons, inputAreas);
    return comment;
}

function attachCommentReplyEvents(replyButtons) {
    replyButtons.off('click');
    replyButtons.on('click', function (e) {
        e.preventDefault();
        requestReplyForm($(this).parent());
        return false;
    });
}

function attachCommentEditEvents(editButtons, inputAreas) {
    const callBack = function (inputArea) { submitCommentEdit(inputArea) }
    editButtons.off('click');
    editButtons.on('click', function (e) {
        e.preventDefault();
        const form = $(this).parent();
        revealEditCommentForm(form, callBack);
        return false;
    });
}

function revealEditCommentForm(form, callBack) {
    const comment = form.parent().parent();
    const commentContent = comment.children('.comment-content');
    commentContent.hide();

    form = form.clone();
    comment.prepend(form);
    const formInput = form.children('.content-input');
    const editButton = form.children('.edit-button');
    editButton.hide();
    formInput.show();
    attachCommentSubmit(formInput, callBack);
    formInput.focus();
}

function requestNewCommentForm(form, commentParent) {
    if (commentParent == null) {
        commentParent = $('#ProfileComments').children('#comments-body');
    }
    data = form.serialize();
    $.ajax({
        type: "POST",
        url: '/Comment/New',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dataType: "html",
        success: function (res) {
            const newCommentForm = createObjectFromHtml(res, 'form');
            commentParent.prepend(newCommentForm);
            const callBack = function (inputArea) { submitNewComment(inputArea.parent(), commentParent) };
            const input = newCommentForm.children('.content-input');
            attachCommentSubmit(input, callBack);
            input.focus();
        },
        error: function (ts) {
            console.log("error", ts);
        }
    });
}

function submitNewComment(form, commentParent) {
    const data = form.serialize();
    form.remove();
    $.ajax({
        type: "POST",
        url: '/Comment/Add',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dataType: "html",
        success: function (res) {
            const comment = buildComment(res);
            commentParent.prepend(comment);
        },
        error: function (ts) { console.log('error', ts)}
    });
}

function submitCommentEdit(inputArea) {
    let form = inputArea.parent();
    const originalComment = form.parent();
    const data = form.serialize();
    form.remove();
    $.ajax({
        type: "POST",
        url: '/Comment/Update',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dateType: "html",
        success: function (res) {
            const revisedComment = buildComment(res);
            const replies = originalComment.children('.replies').children();
            revisedComment.insertBefore(originalComment);
            revisedComment.children('.replies').append(replies);
            originalComment.remove();
        },
        error: function (ts) {
            originalComment.children('.comment-content').show();
            console.log('error:', ts)
        },
    });
}

$(document).ready(function () {
    $('#ProfileImages .updatable').click(function () {
        const modal = $('#modal-container .modal-content');
        const select = this;
        const type = $(this).attr('id');
        const addClass = 'main-alt-select';
        const callBack = function () {
            changeUserImage(modal, type, addClass)
        };
        loadCharacterModal(modal, select, 1, callBack, addClass);
    });

    $('#UpdateUser').click(function (e) {
        e.preventDefault();
        const data = $('#UserUpdateForm').serialize();
        $.ajax({
            type: "POST",
            url: '/ApplicationUser/Update',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: data,
            dataType: "json",
            success: function () { alert('Success'); },
            error: function (ts) { }
        });
        $(this).hide();
        return false;
    });

    $('#AddFriend').click(function (e) {
        e.preventDefault();
        const data = $('#AddFriendForm').serialize();
        $.ajax({
            type: "POST",
            url: '/ApplicationUser/AddFriend',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: data,
            dataType: "json",
            success: function () { alert('Success'); },
            error: function (ts) { }
        });
        $(this).remove();
        return false;
    });

    $('.accept-friend').click(function (e) {
        e.preventDefault();
        const data = $(this).parent().serialize();
        $.ajax({
            type: "POST",
            url: '/ApplicationUser/AcceptFriend',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: data,
            dataType: "json",
            success: function () { alert('Success'); },
            error: function (ts) { }
        });
    });

    $('#NewComment').off('click');
    $('#NewComment').on('click', function (e) {
        e.preventDefault();
        const form = $(this).parent();
        requestNewCommentForm(form);
        return false;
    });

    const replyButtons = $('.NewCommentForm .comment-reply');
    attachCommentReplyEvents(replyButtons);

    const commentEditButtons = $('.EditCommentForm .edit-button');
    const commentInputAreas = $('.EditCommentForm .content-input');
    attachCommentEditEvents(commentEditButtons, commentInputAreas);
});

