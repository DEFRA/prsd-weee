(function () {
    "use strict";

    const $sessionDialog = $("#prsd-timeout-dialog, .prsd-timeout-overlay");
    let defaultTimeOutInMinutes;
    let warningTimeInMinutes;
    let sessionTimeoutInSeconds;
    let warningTimeInSeconds;
    let sessionWarningTimer;
    let sessionLogoutTimer;
    let countdownTimer;
    let urlPrefix = "";
    let eventBound = false;
    let sessionStartTime; // Keep track of the session start time

    // Generate URL based on the environment and URL prefix
    function getUrl(endpoint) {
        const baseUrl = location.protocol + '//' + location.host;
        const prefix = urlPrefix || '';
        const path = location.pathname.split('/')[1];

        return location.host.includes('uat')
            ? `${baseUrl}/${path}/${prefix}${endpoint}`
            : `${baseUrl}/${prefix}${endpoint}`;
    }

    function startSessionTimeout() {
        clearTimeout(sessionWarningTimer);
        clearTimeout(sessionLogoutTimer);

        sessionStartTime = Date.now(); // Record the actual start time of the session
        sessionWarningTimer = setTimeout(showSessionWarning, (sessionTimeoutInSeconds - warningTimeInSeconds) * 1000);
        sessionLogoutTimer = setTimeout(logout, sessionTimeoutInSeconds * 1000);

        startCountdown();
    }

    function showSessionWarning() {
        clearTimeout(countdownTimer);
        setTimeValue(warningTimeInSeconds - 1);
        $sessionDialog.show();
    }

    function startCountdown() {
        countdownTimer = setInterval(function () {
            // Calculate the remaining time by comparing current time with session start time
            const elapsedTimeInSeconds = Math.floor((Date.now() - sessionStartTime) / 1000);
            const remainingTimeInSeconds = sessionTimeoutInSeconds - elapsedTimeInSeconds;

            if (remainingTimeInSeconds <= 0) {
                clearInterval(countdownTimer); // Stop the countdown once the session expires
                logout(); // Trigger logout
            } else {
                setTimeValue(remainingTimeInSeconds);
            }
        }, 1000); // Update every second
    }

    function setTimeValue(remainingTimeInSeconds) {
        $("#prsd-timeout-countdown").html(formatTime(remainingTimeInSeconds));
    }

    function closeSessionWarning() {
        clearTimeout(countdownTimer);
        $sessionDialog.hide();
    }

    async function post(url) {
        const token = $('input[name="__RequestVerificationToken"]').val();
        const formData = new URLSearchParams();
        formData.append("__RequestVerificationToken", token);

        try {
            await $.post({
                url,
                data: formData.toString(),
                contentType: 'application/x-www-form-urlencoded;charset=UTF-8'
            });
        } catch (error) {
            console.error('Error posting data', error);
        }
    }

    async function logout() {
        const logOffUrl = getUrl('/Account/SignOut');
        const signOutUrl = getUrl('/Account/SessionSignedOut');
        await post(logOffUrl);
        document.location.href = signOutUrl;
    }

    function setTimeWith(ltTimeoutInMinutes, lWarningTimeInMinutes) {
        defaultTimeOutInMinutes = ltTimeoutInMinutes;
        warningTimeInMinutes = lWarningTimeInMinutes;

        sessionTimeoutInSeconds = defaultTimeOutInMinutes * 60;
        warningTimeInSeconds = warningTimeInMinutes * 60;
    }

    function formatTime(seconds) {
        const mins = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return mins > 0 ? `${mins} minute(s) and ${secs} second(s)` : `${secs} second(s)`;
    }

    function start(timeoutInMinutes, warningBeforeInMinutes, authenticated, isInternal) {
        if (authenticated === "False") return;

        isInternal = isInternal === "True";
        setTimeWith(timeoutInMinutes, warningBeforeInMinutes);
        startSessionTimeout();

        $("#signoutForm").toggle(!isInternal);
        $("#signoutFormAdmin").toggle(isInternal);

        if (!eventBound) {
            $("#prsd-timeout-keep-signin-btn").click(async () => {
                const extendSessionUrl = getUrl('/Account/ExtendSession');
                await post(extendSessionUrl);

                clearTimeout(sessionWarningTimer);
                clearTimeout(sessionLogoutTimer);
                clearInterval(countdownTimer); // Clear the old countdown

                sessionStartTime = Date.now(); // Reset session start time
                startSessionTimeout();
                closeSessionWarning();
            });

            eventBound = true;
        }
    }

    window.weee = window.weee || {};
    window.weee.sessionTimeout = window.weee.sessionTimeout || {};
    window.weee.sessionTimeout.start = start;
}).call(this);
