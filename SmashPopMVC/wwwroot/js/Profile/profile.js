function changeUserImage(modal, mainOrAlt, modalClass) {
    if (changeCharacterCard(modal, mainOrAlt, modalClass)) {
        $('#UpdateUser').show();
    }
}

function requestNewCommentForm(form, commentParent) {
    data = form.serialize();
    $.ajax({
        type: "POST",
        url: '/Comment/New',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (res) {
            const res_object = $('#ProfileComments #comment-temp').html(res);
            const form = res_object.children('form');
            form.on("keydown", ":input:not(textarea)", function (e) {
                return event.key != "Enter";
            });
            form.on("keydown", "textarea", function (e) {
                if (e.key == "Enter") { submitComment(form, commentParent); }
            });
            form.children('.content-input').blur(function (e) { submitComment(form, commentParent) });
            commentParent.prepend(res_object.children());
            res_object.html('');
        },
        error: function (ts) {
            console.log("error", ts);
        }
    });
}

function submitComment(form, commentParent) {
    const data = form.serialize();
    form.remove();
    $.ajax({
        type: "POST",
        url: '/Comment/Add',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dataType: "html",
        success: function (res) {
            const res_object = $('#ProfileComments #comment-temp').html(res);
            const form = res_object.children('form');
            form.children('.content-input').blur(function (e) { submitComment(form) });
            commentParent.prepend(res_object.children());
            res_object.html('');
        },
        error: function (ts) { console.log('error', ts)}
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

    $('#NewComment').click(function (e) {
        console.log('new');
        e.preventDefault();
        const form = $(this).parent();
        const commentParent = $('#ProfileComments').children('#comments-body');
        requestNewCommentForm(form, commentParent);
        return false;
    });

    $('.comment-reply').click(function (e) {
        console.log('reply');
        e.preventDefault();
        const form = $(this).parent();
        const commentParent = form.parent().siblings('.replies');
        requestNewCommentForm(form, commentParent);
        return false;
    });
});

