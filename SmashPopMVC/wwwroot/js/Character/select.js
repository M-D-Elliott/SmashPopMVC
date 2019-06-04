let animate = true;

function loadCharacterModal(modal, initialSelect, maxSelect, submitCallBack, modalClass) {
    if (modalClass === undefined) {
        modalClass = "";
    }
    const select = $(initialSelect).clone();
    animate = true;
    select.removeClass('modal-link updatable col-6');
    select.removeAttr('id');
    modal.addClass(modalClass);
    if ($.trim(modal.html()) == '' || !(modal.find('#Characters').length)) {
        $.ajax({
            type: "GET",
            url: '/Character/Select',
            success: function (res) {
                modal.html(res);
                const characters = modal.find('#Characters .card');
                characters.click(function () { addToSelected($(this).clone(), maxSelect) })
                modal.on('click', function () {
                    endAnimateCharacterCard();
                });
                startCharacterModal(modal, select, maxSelect, submitCallBack);
            },
        });
    } else {
        startCharacterModal(modal, select, maxSelect, submitCallBack);
    }
}

function startCharacterModal(modal, select, maxSelect, submitCallBack) {
    addToSelected(select, maxSelect);
    submitButton = modal.find('#SubmitButton');
    submitButton.off('click');
    submitButton.on('click', function () {
        submitCallBack()
        modal.find('.modal-close-btn').click();
    });
    
    const firstCharacter = modal.find('#Characters .card:first-child');
    firstCharacter.addClass('come-in show');
    animateCharacterCard(firstCharacter);
}

function endAnimateCharacterCard() {
    animate = false;
}

function animateCharacterCard(card) {
    card.one('webkitAnimationEnd oanimationend msAnimationEnd animationend', function () {
        const character = $(this);
        character.removeClass('come-in');
        if(animate) {
            if(!character.is(':last-child')) {
                const nextCharacter = character.next();
                nextCharacter.addClass('come-in show');
                animateCharacterCard(nextCharacter);
            }
        } else {
            characters = character.nextAll();
            characters.addClass('show');
        }
    });
}

function addToSelected(charCard, max) {
    if (max === undefined) {
        max = 5;
    }
    const selectedCharacters = $('#SelectedCharacters');
    selectedCharacters.children().each(function () {
        if ($(this).attr('title') == charCard.attr('title')) {
            $(this).remove();
        }
    });
    charCard.addClass('relative-parent');
    charCard.removeClass('clickable-card');
    charCard.prepend('<button type="button" class="btn grey-white ml-0 p-0"><i class="fa fa-window-close p-0 m-0"></i></button>');
    charCard.children('button').click(function () { removeFromSelected($(this)); });
    selectedCharacters.append(charCard);
    if (selectedCharacters.children().length > max) {
        selectedCharacters.children('div:first').remove()
    }
}

function removeFromSelected(target) {
    target.parent().remove();
}

function changeCharacterCard(modal, type, modalClass) {
    if (modalClass === undefined) {
        modalClass = '';
    }
    const characterCard = $('#' + type);
    const selectedCharacter = modal.find('#SelectedCharacters div:first-child img');
    const hiddenInput = $('#' + type + 'IDInput');
    const newID = selectedCharacter.attr('data-id');
    console.log(hiddenInput.val(), newID);
    if (selectedCharacter.length > 0 && hiddenInput.val() !== newID) {
        hiddenInput.val(newID);
        characterCard.children().remove();
        characterCard.append(selectedCharacter.clone());
        modal.removeClass(modalClass);
        return true;
    }
    return false;
}

$(document).ready(function () {
    $("#modal-container").on('hidden.bs.modal', function () {
        $(this).find('#Characters .card').removeClass('show');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    });
});