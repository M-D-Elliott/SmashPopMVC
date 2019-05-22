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

    selectedCharacters.append(charCard);
    if (selectedCharacters.children().length > max) {
        selectedCharacters.children('div:first').remove()
    }
}