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

function requestNewCommentForm(form, commentParent) {
    data = form.serialize();
    $.ajax({
        type: "POST",
        url: '/Comment/New',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dataType: "html",
        success: function (res) {
            const newCommentForm = createObjectFromHtml(res, 'form');
            newCommentForm.on("keydown", ":input:not(textarea)", function (e) {
                return event.key != "Enter";
            });
            newCommentForm.on("keydown", "textarea", function (e) {
                if (e.key == "Enter") { submitNewComment(newCommentForm, commentParent); }
            });
            newCommentForm.children('.content-input').blur(function (e) { submitNewComment(newCommentForm, commentParent) });
            commentParent.prepend(newCommentForm);
        },
        error: function (ts) {
            console.log("error", ts);
        }
    });
}

function requestReplyForm(form) {
    const commentParent = form.parent().siblings('.replies');
    requestNewCommentForm(form, commentParent);
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
            const comment = createObjectFromHtml(res, '.comment');
            const replyForm = comment.children('.comment-links').children('.NewCommentForm');
            const replyButton = replyForm.children('.comment-reply');
            replyButton.off('click');
            replyButton.on('click', function (e) {
                e.preventDefault();
                console.log('yes');
                requestReplyForm($(this).parent());
                return false;
            });
            commentParent.prepend(comment);
        },
        error: function (ts) { console.log('error', ts)}
    });
}

function revealEditCommentForm(form) {
    form.children('.title-input, .content-input').show();
    form.parent().children('.comment-title, .comment-content').hide();
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
        const commentParent = $('#ProfileComments').children('#comments-body');
        requestNewCommentForm(form, commentParent);
        return false;
    });

    $('.comment-reply').off('click');
    $('.comment-reply').on('click', function (e) {
        e.preventDefault();
        requestReplyForm($(this).parent());
        return false;
    });

    $('.edit').off('click');
    $('.edit').on('click', function () {
        e.preventDefault();
        const form = $(this).parent();
        revealEditCommentForm(form);
    });

    $('.EditCommentForm').children('.content-input').blur(function (e) {
        submitCommentEdit();
    });
});

