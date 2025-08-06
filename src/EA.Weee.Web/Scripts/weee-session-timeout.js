(function () {
    "use strict";

    let defaultTimeOutInMinutes;
    let warningTimeInMinutes;

    let sessionTimeoutInSeconds;
    let warningTimeInSeconds;

    let sessionWarningTimer;
    let sessionLogoutTimer;
    let countdownTimer;

    let urlPrefix = "";
    let eventBound = false;

    const $sessionDialog = $("#prsd-timeout-dialog, .prsd-timeout-overlay");

    function startSessionTimeout() {

        clearTimeout(sessionWarningTimer);
        clearTimeout(sessionLogoutTimer);

        sessionWarningTimer = setTimeout(() => showSessionWarning(), (sessionTimeoutInSeconds - warningTimeInSeconds) * 1000);
        sessionLogoutTimer = setTimeout(() => logout(), sessionTimeoutInSeconds * 1000);
    }

    function showSessionWarning() {

        clearTimeout(countdownTimer); // Stop any previous countdown
        setTimeValue(warningTimeInSeconds - 1);
        $sessionDialog.show();
    }

    function setTimeValue(val) {

        let setVal = () => $("#prsd-timeout-countdown").html(formatTime(val));
        setVal();

        if (val < 1)
            return;

        countdownTimer = setTimeout(() => {
            setTimeValue(--val);
            setVal();
        }, 1000);
    }

    function closeSessionWarning() {

        clearTimeout(countdownTimer); // Stop countdown when modal is closed
        $sessionDialog.hide();
    }

    async function post(url) {

        let tokenName = "__RequestVerificationToken";
        let token = $('input[name=' + tokenName + ']').val();

        let formData = new URLSearchParams();
        formData.append(tokenName, token)

        await $.post({
            url: url,
            data: formData.toString(),
            contentType: 'application/x-www-form-urlencoded;charset=UTF-8'
        });
    }

    async function logout() {

        let signOutUrl = null;
        let logOffUrl = null;

        if (location.host.includes('uat')) {
            logOffUrl = location.protocol + '//' + location.host + '/' + location.pathname.split('/')[1] + '/Account/SignOut';
            signOutUrl = location.protocol + '//' + location.host + '/' + location.pathname.split('/')[1] + '/Account/SessionSignedOut';
        }
        else {
            logOffUrl = location.protocol + '//' + location.host + '/Account/SignOut';
            //logOffUrl = '/Account/SignOut';
            signOutUrl = location.protocol + '//' + location.host + '/Account/SessionSignedOut';
        }

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

        let mins = Math.floor(seconds / 60);
        let secs = seconds % 60;
        let minutesResult = mins > 0 ? mins + ' minute(s) and ' + secs + ' seconds' : + secs + ' seconds';

        return minutesResult;
    }

    function start(timeoutInMinutes, warningBeforeInMinutes, authenticated, isInternal) {

        if (authenticated == "False")
            return;

        isInternal = isInternal == "True";

        setTimeWith(timeoutInMinutes, warningBeforeInMinutes);
        startSessionTimeout();

        if (isInternal) {
            urlPrefix = "/Admin"
            $("#signoutForm").hide();
            $("#signoutFormAdmin").show();
        }
        else {
            $("#signoutForm").show();
            $("#signoutFormAdmin").hide();
        }

        if (!eventBound) {
            $("#prsd-timeout-keep-signin-btn").click(async () => {
                let extendSessionUrl = null;
                if (location.host.includes('uat')) {
                    if (urlPrefix == null) {
                        extendSessionUrl = location.protocol + '//' + location.host + '/' + location.pathname.split('/')[1] + '/Account/ExtendSession';
                    }
                    else {
                        extendSessionUrl = location.protocol + '//' + location.host + '/' + location.pathname.split('/')[1] + '/' + urlPrefix + '/Account/ExtendSession';
                    }
                }
                else {
                    if (urlPrefix == null) {
                        extendSessionUrl = location.protocol + '//' + location.host + '/Account/ExtendSession';
                    }
                    else {
                        extendSessionUrl = location.protocol + '//' + location.host + '/' + urlPrefix + '/Account/ExtendSession';
                    }

                }
                await post(extendSessionUrl);

                clearTimeout(sessionWarningTimer);
                clearTimeout(sessionLogoutTimer);
                clearTimeout(countdownTimer);

                setTimeWith(timeoutInMinutes, warningBeforeInMinutes);
                startSessionTimeout();
                closeSessionWarning();
            });
        }
    }

    window.weee = window.weee || {};
    window.weee.sessionTimeout = window.weee.sessionTimeout || {};
    window.weee.sessionTimeout.start = start;
}).call(this);
