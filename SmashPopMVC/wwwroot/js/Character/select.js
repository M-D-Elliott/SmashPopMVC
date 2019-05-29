$("#modal-container").modal('hide').on('hidden.bs.modal', function () {
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    $('#modal-content').removeAttr('data-maxSelect');
});

$('#modal-container').on("click", '.character', function () {
    addToSelected(
        $(this).parent().clone(),
        max = $('#modal-container .modal-content').data('maxSelect'));
});

function addToSelected(charCard, max = 5) {
    const selectedCharacters = $('#SelectedCharacters');

    selectedCharacters.children().each(function () {
        if ($(this).attr('title') == charCard.attr('title')) {
            $(this).remove();
        }
    });
    charCard.addClass('relative-parent');
    charCard.prepend('<button type="button" class="btn grey-white ml-0 p-0"><i class="fa fa-window-close p-0 m-0"></i></button>');
    charCard.children('button').click(function (e) {removeFromSelected($(e.target)); });
    selectedCharacters.append(charCard);
    if (selectedCharacters.children().length > max) {
        selectedCharacters.children('div:first').remove()
    }
}

function removeFromSelected(target) {
    let charCard = target.parent();
    if (target.is('i')) {
        charCard = charCard.parent();
    }
    charCard.remove();
}