﻿
function InitialStartup(openAllText, closeAllText) {
    var summaries = document.querySelectorAll(".govuk-details__summary");

    for (var i = 0; i < summaries.length; i++) {
        summaries[i].addEventListener("click",
            function (e) {
                function allOpenClosedCheck(id, valueToCheck) {
                    for (var i = 0; i < summaries.length; i++) {
                        var detailsElement = summaries[i];
                        if (detailsElement.id !== id) {
                            if (detailsElement.getAttribute("aria-expanded") !== valueToCheck) {
                                return false;
                            }
                        }
                    }
                    return true;
                }

                var isOpening = false;
                if (e.target.getAttribute("aria-expanded") === "true") {
                    isOpening = true;
                    e.target.nextElementSibling.style.display = "";
                }

                if (isOpening && allOpenClosedCheck(e.target.id, "true") === true) {
                    document.getElementById("collapsibleHref").innerText = closeAllText;
                } else {
                    document.getElementById("collapsibleHref").innerText = closeAllText;
                }

                if (!isOpening && allOpenClosedCheck(e.target.id, "false") === true) {
                    document.getElementById("collapsibleHref").innerText = openAllText;
                }
            });
    }

    var open = document.getElementById("collapsibleHref");

    if (open) {
        open.addEventListener("click",
            function (e) {
                e.preventDefault();
                var ua = window.navigator.userAgent;
                var isIEorEdge = /MSIE|Trident/.test(ua) || ua.indexOf("Edge") > -1;

                var details = document.querySelectorAll(".govuk-details");
                var openAll = false;
                if (document.getElementById("collapsibleHref").innerText === openAllText) {
                    document.getElementById("collapsibleHref").innerText = closeAllText;
                    openAll = true;
                } else {
                    document.getElementById("collapsibleHref").innerText = openAllText;
                    openAll = false;
                }

                for (var i = 0; i < details.length; i++) {
                    var detailsElement = details[i];
                    if (openAll) {
                        detailsElement.open = "open";
                        detailsElement.getElementsByClassName("govuk-details__summary")[0].setAttribute("aria-expanded", true);
                        detailsElement.getElementsByClassName("govuk-details__text")[0].setAttribute("aria-hidden", false);
                        if (isIEorEdge) {
                            detailsElement.getElementsByClassName("govuk-details__text")[0].style.display = "";
                            detailsElement.setAttribute("open", "");
                        }
                    } else {
                        detailsElement.removeAttribute("open");
                        detailsElement.getElementsByClassName("govuk-details__summary")[0].setAttribute("aria-expanded", false);
                        detailsElement.getElementsByClassName("govuk-details__text")[0].setAttribute("aria-hidden", true);
                        if (isIEorEdge) {
                            detailsElement.getElementsByClassName("govuk-details__text")[0].style.display = "none";
                        }
                    }
                }
            });
    }
}