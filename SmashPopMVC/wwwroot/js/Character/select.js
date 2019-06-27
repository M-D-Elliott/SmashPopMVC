let animate = true;

function sendAjaxRequest(url, form, dataType, successCallBack, errorCallBack, type) {
    if (type == undefined || type == null) {
        type = "POST";
    }
    if (dataType == undefined || dataType == null) {
        dataType = "json";
    }
    if (successCallBack == undefined || dataType == null) {
        successCallBack = function (res) { }
    }
    if (errorCallBack == undefined || errorCallBack == null) {
        try {
            errorCallBack = function (ts) { swal("Error!", ts.responseText, "warning"); };
        }
        catch (err) {
            errorCallBack = function (ts) { console.log(ts.error); };
        }
    }
    const data = form == null ? null : form.serialize();
    $.ajax({
        type: type,
        url: url,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        dataType: dataType,
        success: function (res) { successCallBack(res, form); },
        error: function (ts) { errorCallBack(ts); }
    });
}

function loadCharacterModal(modal, initialSelect, maxSelect, submitCallBack, modalClass) {
    if ($.trim(modal.html()) == '' || !(modal.find('#Characters').length)) {
        const successCallBack = function (res, form) {
            startCharacterModal(modal, res, initialSelect, maxSelect, submitCallBack, modalClass);
        };
        sendAjaxRequest('/Character/Select', null, 'html', successCallBack, null, 'GET')
    } else {
        startCharacterModal(modal, null, initialSelect, maxSelect, submitCallBack, modalClass);
    }
}

function startCharacterModal(modal, res, initialSelect, maxSelect, submitCallBack, modalClass) {
    animate = true;
    if (modalClass != undefined) {
        modal.addClass(modalClass);
    }
    if (res != undefined && res != null) {
        modal.html(res);
    }
    const select = $(initialSelect).clone();
    select.removeClass('modal-link updatable col-6');
    
    const characters = modal.find('#Characters .card');
    characters.click(function () { addToSelected($(this).children('img').clone(), maxSelect) })
    modal.on('click', function () {
        endAnimateCharacterCard();
    });
    addToSelected(select.children('img').clone(), maxSelect);
    const select_type = select.attr('id');
    select.removeAttr('id');
    modal.children('.modal-body').children('#ChooseYourCharacter').text("Choose your " + select_type + " Character!");
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