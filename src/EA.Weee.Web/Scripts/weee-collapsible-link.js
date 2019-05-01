function CollapsibleLinkHref() {
    if (document.getElementById('collapsibleHref').innerText == "Open all") {
        var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
        for (i = 0; i < schemaData.length; i++) {
            document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = "inherit";
            var divopen = document.getElementsByClassName('govuk-details')[i].open;
            if (divopen) {
                document.getElementsByClassName('govuk-details')[i].open = false
            }
            else if (!divopen) {
                document.getElementsByClassName('govuk-details')[i].open = true;
            }
        }
        document.getElementById('collapsibleHref').innerText = "Close all";
    }
    else if (document.getElementById('collapsibleHref').innerText == "Close all") {
        var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
        for (i = 0; i < schemaData.length; i++) {
            var cdisplay = document.getElementsByClassName('collapsible-govuk-grid')[i].style.display;
            document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = 'none';
            var copen = document.getElementsByClassName('govuk-details')[i].open;
            document.getElementsByClassName('govuk-details')[i].open = false;
        }
        document.getElementById('collapsibleHref').innerText = "Open all";
    }
}

function CollapsibleLinkHefSummary() {
    var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
    for (i = 0; i < schemaData.length; i++) {
        document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = "inherit";
    }

    SetcollapsibleHrefText();
}

function SetcollapsibleHrefText() {
    var elements = document.getElementsByClassName('govuk-details');

    var closedElements = [];

    for (i = 0; i < elements.length; i++) {


        var x = elements[i].open;

        closedElements[i] = x;

    }

    var x = false;
    console.log(closedElements);

    if (new Set(closedElements).size == 1 && closedElements[0] == true) {
        document.getElementById('collapsibleHref').innerText = "Open all";
    } else {
        document.getElementById('collapsibleHref').innerText = "Close all";
    }
}

function InitialStartup() {
    var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
    for (i = 0; i < schemaData.length; i++) {
        document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = 'none';
        document.getElementsByClassName('govuk-details')[i].open = false;
    }
    document.getElementById('collapsibleHref').innerText = "Open all";
}