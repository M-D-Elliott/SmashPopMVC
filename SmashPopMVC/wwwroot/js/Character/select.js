let animate = true;

function loadCharacterModal(modal, initialSelect, maxSelect, submitCallBack, modalClass) {
    if (modalClass === undefined) {
        modalClass = "";
    }
    const select = $(initialSelect).clone();
    animate = true;
    select.removeClass('modal-link updatable col-6');
    modal.addClass(modalClass);
    if ($.trim(modal.html()) == '' || !(modal.find('#Characters').length)) {
        $.ajax({
            type: "GET",
            url: '/Character/Select',
            success: function (res) {
                modal.html(res);
                const characters = modal.find('#Characters .card');
                characters.click(function () { addToSelected($(this).children('img').clone(), maxSelect) })
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
    addToSelected(select.children('img').clone(), maxSelect);
    const select_type = select.attr('id');
    select.removeAttr('id');
    modal.children('.modal-body').children('#ChooseYourCharacter').text(`Choose your ${select_type} Character!`);
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

function addToSelected(img, max) {
    if (max === undefined) {
        max = 5;
    }
    const selectedCharacters = $('#SelectedCharacters');
    let charCard = selectedCharacters.siblings('#emptyCharCard')
        .clone()
        .css({ "display": "block" })
        .append(img);
    selectedCharacters.children().each(function () {
        if ($(this).attr('title') == charCard.attr('title')) {
            $(this).remove();
        }
    });
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
    if (modalClass !== undefined) {
        modal.removeClass(modalClass);
    }
    const characterCard = $('#' + type);
    const originalImage = characterCard.children('img');
    const newImage = modal.find('#SelectedCharacters div:first-child img');
    const hiddenInput = $('#' + type + 'IDInput');
    const newID = newImage.attr('data-id');
    
    if (newImage.length > 0 && hiddenInput.val() !== newID) {
        characterCard.children('.card-title').text(newImage.attr('title'));
        hiddenInput.val(newID);
        newImage
            .insertBefore(originalImage);
        originalImage.remove();
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