function setUserVote(modal, type, modalClass = '') {
    changeCharacterCard(modal, type, modalClass);
}

$(document).ready(function () {
    $('.voteIcon').click(function () {
        const modal = $('#modal-container .modal-content');
        const select = this;
        const type = $(this).attr('id');
        const addClass = 'vote-select';
        const callBack = function () {
            setUserVote(modal, type, modalClass = addClass);
        };
        loadCharacterModal(modal, select, 1, callBack, modalClass = addClass);
    });
});