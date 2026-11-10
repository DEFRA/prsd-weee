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

    let eventBound = false;
    let sessionStartTime;
    let isLoggingOut = false;

    function getUrl(endpoint) {
        const baseUrl = location.protocol + "//" + location.host;
        const prefix = urlPrefix || "";
        const path = location.pathname.split("/")[1];

        return location.host.includes("uat")
            ? `${baseUrl}/${path}/${prefix}${endpoint}`
            : `${baseUrl}/${prefix}${endpoint}`;
    }

    function startSessionTimeout() {
        clearTimeout(sessionWarningTimer);
        clearTimeout(sessionLogoutTimer);
        clearInterval(countdownTimer);

        isLoggingOut = false;
        sessionStartTime = Date.now();

        sessionWarningTimer = setTimeout(
            showSessionWarning,
            (sessionTimeoutInSeconds - warningTimeInSeconds) * 1000
        );

        sessionLogoutTimer = setTimeout(
            logout,
            sessionTimeoutInSeconds * 1000
        );
    }

    function showSessionWarning() {
        sessionStartTime = Date.now();

        setTimeValue(warningTimeInSeconds);
        $sessionDialog.show();

        startCountdown();
    }

    function startCountdown() {
        clearInterval(countdownTimer);

        countdownTimer = setInterval(function () {
            const elapsed = Math.floor((Date.now() - sessionStartTime) / 1000);
            const remaining = warningTimeInSeconds - elapsed;

            if (remaining <= 0) {
                clearInterval(countdownTimer);
                logout();
            } else {
                setTimeValue(remaining);
            }
        }, 1000);
    }

    function setTimeValue(remainingTimeInSeconds) {
        $("#prsd-timeout-countdown").text(formatTime(remainingTimeInSeconds));
    }

    function closeSessionWarning() {
        clearInterval(countdownTimer);
        $sessionDialog.hide();
    }

    async function post(url) {
        const token = $('input[name="__RequestVerificationToken"]').first().val();

        if (!token) {
            console.error("Anti-forgery token is missing.");
            return false;
        }

        try {
            await $.ajax({
                url: url,
                type: "POST",
                data: {
                    __RequestVerificationToken: token
                },
                contentType: "application/x-www-form-urlencoded; charset=UTF-8"
            });

            return true;
        } catch (error) {
            console.error("Error posting data", error);
            return false;
        }
    }

    async function logout() {
        if (isLoggingOut) return;

        isLoggingOut = true;

        clearTimeout(sessionWarningTimer);
        clearTimeout(sessionLogoutTimer);
        clearInterval(countdownTimer);

        const logOffUrl = getUrl("Account/SignOut");
        const signOutUrl = getUrl("Account/SessionSignedOut");

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
        if (!authenticated || authenticated === "False") return;

        isInternal = isInternal === "True";
        setTimeWith(timeoutInMinutes, warningBeforeInMinutes);

        startSessionTimeout();

        $("#signoutForm").toggle(!isInternal);
        $("#signoutFormAdmin").toggle(isInternal);

        if (!eventBound) {
            $("#prsd-timeout-keep-signin-btn").on(
                "click",
                async function (event) {
                    event.preventDefault();

                    const extendSessionUrl = getUrl("Account/ExtendSession");

                    const success = await post(extendSessionUrl);

                    if (!success) {
                        return;
                    }

                    closeSessionWarning();
                    startSessionTimeout();
                }
            );

            eventBound = true;
        }
    }

    window.weee = window.weee || {};
    window.weee.sessionTimeout = window.weee.sessionTimeout || {};
    window.weee.sessionTimeout.start = start;
}).call(this);
