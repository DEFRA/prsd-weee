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
                document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = 'none';
                document.getElementsByClassName('govuk-details')[i].open = false;             
            }
            document.getElementById('collapsibleHref').innerText = "Open all";
        }
}

function CheckLinksAreClosed() {

    var linkclosed = false;
    var collapsibleElements = document.getElementsByClassName('govuk-details');

    for (i = 0; i < collapsibleElements.length; i++) {
        var divopen = document.getElementsByClassName('govuk-details')[i].open;

        if (!divopen) {
            linkclosed = true;
        }
        else if (divopen) {
            linkclosed = false;
        }
    }
    return linkclosed;
}

function CollapsibleLinkHefSummary() {
    var linksClosed = CheckLinksAreClosed();
    if (linksClosed) {
        alert("linksClosed " + linksClosed);
        document.getElementById('collapsibleHref').innerText = "Open all";
    }

    var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
    for (i = 0; i < schemaData.length; i++) {
        document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = "inherit";   
    }
    document.getElementById('collapsibleHref').innerText = "Close all";
}

function InitialStartup() {
    var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
    for (i = 0; i < schemaData.length; i++) {
        document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = 'none';
        document.getElementsByClassName('govuk-details')[i].open = false;
    }
    document.getElementById('collapsibleHref').innerText = "Open all";
}