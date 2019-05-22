function getRandomDictItem(dict, n) {
    randomID = Math.floor(Math.random() * n);
    return dict[randomID];
}

function getRandomTime(min, max) {
    return Math.round(Math.random() * (max - min) + min);
}

function getRandomCoords(x_max, y_max) {
    var result = {
        x: Math.floor(Math.random() * (x_max)),
        y: Math.floor(Math.random() * (y_max)),
    };
    return result;
}

function IconData(character) {
    if (character.origin != null) {
        this.originID = character.origin.id;
        this.originTitle = character.origin.title;
        this.originYear = character.origin.year;
    }
    this.smashOriginID = character.smashOrigin.id;
    this.smashOriginTitle = character.smashOrigin.title;
}

let numChars = Object.keys(whackCharacters).length;
const gameArea = $('#GameArea');
const scoreBoard = $('#Score');
let score = parseInt(scoreBoard.text());
const timeBoard = $('#Time');
const comboBoard = $('#Combo');
let comboList = [];
let comboName = '';
let iconTrans = 'shrink';

const startButton = $('#Start')
const endButton = $('#End')

let w = parseInt(gameArea.css('width'));
let h = parseInt(gameArea.css('height'));
let gameOver = false;

const levelBoard = $('#Level');
let level = 7;
const spawnRate = 1500;
const shrinkRate = 40;

let gameDuration = 20000;
let timeUp = true;
let levelStart = 0;
let levelDelta = 0;


function updateTimer(add = 0) {
    levelDelta = new Date().getTime() - levelStart
    if (levelDelta > gameDuration) {
        return endGame()
    } else {
        timeBoard.text(gameDuration - levelDelta);
    }
}

function produceIcon(character, coords) {
    const icon = $('<div class="whack-icon character ' + character.imageName + '">');
    const iconData = IconData.call(this, character);
    icon.data('data', iconData);
    icon
        .offset({ left: coords.x, top: coords.y })
        .appendTo(gameArea);
    icon.click(hit);
    icon.one($.support.transition.end, function () {
        $(this).remove();
    });
    icon.css({ transition: 'all ' + Math.round(shrinkRate / level) + 's' });
    return icon;
}

function loop() {
    const coords = getRandomCoords(w, h);
    const character = getRandomDictItem(whackCharacters, numChars);
    const icon = produceIcon(character, coords);
    updateTimer();
    setTimeout(() => {
        icon.addClass(iconTrans);
        if (!timeUp) loop();
    }, Math.round(spawnRate / level));
}

function calcScore(add = 0) {
    score += add * level;
    scoreBoard.text(score);
}

function calcCombo() {
    return 10;
}

function hit(e) {
    if (!e.hasOwnProperty('originalEvent')) return;
    const icon = $(e.target);
    //console.log(icon.data('data'));
    const bonus = calcCombo();
    calcScore(add = bonus);
    icon.remove();
}

function startGame() {
    //gameArea.click(function () {
    //    $(this).css({ cursor: 'url(images/MiniGames/hammerdrop.png), default'});
    //})
    startButton.hide();
    endButton.show();
    timeBoard.text(gameDuration)
    calcScore();
    timeUp = false;
    levelBoard.text(level);
    levelStart = new Date().getTime();
    loop();
}

function endGame() {
    endButton.hide();
    startButton.show();
    timeBoard.text(0);
    timeUp = true
    gameArea.empty();
    comboBoard.empty();
    score = 0;
    calcScore();
}

console.log(whackCharacters);