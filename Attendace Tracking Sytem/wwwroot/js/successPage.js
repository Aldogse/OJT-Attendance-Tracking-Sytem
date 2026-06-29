let seconds = 5;

const countdown =
    document.getElementById("countdown");

const timer = setInterval(function () {
    seconds--;

    countdown.innerText = seconds;

    if (seconds <= 0) {
        clearInterval(timer);

        window.location.href =
            '@Url.Action("LoginPage", "Account")';
    }

}, 1000);