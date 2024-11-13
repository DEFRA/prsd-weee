﻿(function () {
    "use strict"
    var root = this;
    if (typeof root.GOVUK === 'undefined') {
        root.GOVUK = {};
    }

    /*
      Cookie methods
      ==============
  
      Usage:
  
        Setting a cookie:
        GOVUK.cookie('hobnob', 'tasty', { days: 30 });
  
        Reading a cookie:
        GOVUK.cookie('hobnob');
  
        Deleting a cookie:
        GOVUK.cookie('hobnob', null);
    */
    function getDomain() {
        var host = window.location.hostname;
        if (typeof host !== "undefined" && host !== null && host.length > 0) {
            return host;
        }
        else {
            console.log("The host name is undefined or null!");
            return null;
        }
    }

    GOVUK.cookie = function (name, value, options) {
        if (typeof value !== 'undefined') {
            if (value === false || value === null) {
                return GOVUK.setCookie(name, '', {
                    days: -1
                });
            } else {
                return GOVUK.setCookie(name, value, options);
            }
        } else {
            return GOVUK.getCookie(name);
        }
    };
    GOVUK.setCookie = function (name, value, options) {
        if (typeof options === 'undefined') {
            options = {};
        }
        var currentDomain = getDomain();
        var cookieString = name + "=" + value + ";domain=" + currentDomain + "; path=/";
        if (options.days) {
            var date = new Date();
            date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
            cookieString = cookieString + "; expires=" + date.toGMTString();
        }
        if (document.location.protocol == 'https:') {
            cookieString = cookieString + "; Secure";
        }
        document.cookie = cookieString;
    };
    GOVUK.getCookie = function (name) {
        var nameEQ = name + "=";
        var cookies = document.cookie.split(';');
        for (var i = 0, len = cookies.length; i < len; i++) {
            var cookie = cookies[i];
            while (cookie.charAt(0) == ' ') {
                cookie = cookie.substring(1, cookie.length);
            }
            if (cookie.indexOf(nameEQ) === 0) {
                return decodeURIComponent(cookie.substring(nameEQ.length));
            }
        }
        return null;
    };
}).call(this);

(function () {
    "use strict"
    var root = this;
    if (typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

    GOVUK.addCookieMessage = function () {
        var message = document.getElementById('global-cookie-message'),
            hasCookieMessage = (message && GOVUK.cookie('seen_cookie_message') === null);

        if (hasCookieMessage) {
            message.style.display = 'block';
            GOVUK.cookie('seen_cookie_message', 'yes', { days: 365 });
        }
    };
}).call(this);

(function () {
    "use strict"

    // add cookie message
    if (window.GOVUK && GOVUK.addCookieMessage) {
        GOVUK.addCookieMessage();
    }
}).call(this);



