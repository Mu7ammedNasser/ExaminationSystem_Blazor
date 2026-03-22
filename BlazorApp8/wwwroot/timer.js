window.examTimer = {
    timerInterval: null,
    start: function (durationMinutes, dotnetHelper) {
        let durationSeconds = durationMinutes * 60;
        let display = document.getElementById("timer-display");

        if(this.timerInterval) clearInterval(this.timerInterval);

        this.timerInterval = setInterval(function () {
            let minutes = parseInt(durationSeconds / 60, 10);
            let seconds = parseInt(durationSeconds % 60, 10);

            minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;

            if (display) {
                display.textContent = minutes + ":" + seconds;
            }

            if (--durationSeconds < 0) {
                clearInterval(window.examTimer.timerInterval);
                // Call Blazor C# method to submit exam
                dotnetHelper.invokeMethodAsync('TimeExpired');
            }
        }, 1000);
    },
    stop: function() {
        if(this.timerInterval) {
            clearInterval(this.timerInterval);
        }
    }
};
