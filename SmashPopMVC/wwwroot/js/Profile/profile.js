$('#ProfileImages .updatable').click(function () {
    const mainOrAlt = $(this).attr('id');
    $.ajax({
        type: "GET",
        url: '/Character/Select',
        success: function (res) {
            var modal = $('#modal-container .modal-content');
            modal.addClass('main-alt-select');
            modal.html(res);
            modal.find('#SubmitButton').click(function () { changeUserImages(mainOrAlt) });
            modal.data('maxSelect', 1);
            modal.modal('show');
        },
    });
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
        error: function(ts) { }
    });
    $(this).hide();
    return false;
});

function changeUserImages(mainOrAlt) {
    var modal = $('#modal-container .modal-content');
    selectedCharacter = modal.find('#SelectedCharacters').children();
    $('#' + mainOrAlt + 'ID').val(selectedCharacter.attr('id'));
    let userCharacterCard = $('#' + mainOrAlt)
    userCharacterCard.attr('title', selectedCharacter.attr('title'));
    userCharacterCard.children().remove();
    userCharacterCard.append(selectedCharacter.children('img'))
    $('#UpdateUser').show();
}

$('#AddFriend').click(function (e) {
    e.preventDefault();
    const data = $('#AddFriendForm').serialize();
    console.log(data);
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
    console.log(data);
    $.ajax({
        type: "POST",
        url: '/ApplicationUser/AcceptFriend',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dataType: "json",
        success: function () { alert('Success'); },
        error: function (ts) { }
    })
});