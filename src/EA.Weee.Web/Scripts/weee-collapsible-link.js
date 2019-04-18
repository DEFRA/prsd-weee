function CollapsibleLinkHref() {
    document.getElementById('collapsibleHref').onclick = function () {

        if (document.getElementById('collapsibleHref').innerText == "Open All") {
            var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
            for (i = 0; i < schemaData.length; i++) {
                document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = "inherit";
                document.getElementsByClassName('govuk-details')[i].open = "open";
                document.getElementById('collapsibleHref').innerText = "Close All";
            }
        }
        else if (document.getElementById('collapsibleHref').innerText == "Close All") {
            var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
            for (i = 0; i < schemaData.length; i++) {
                document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = 'none';
                document.getElementsByClassName('govuk-details')[i].open = false;
                document.getElementById('collapsibleHref').innerText = "Open All";
            }
        }
    }
}

function CollapsibleLinkHefSummary() {
    document.getElementById('summary-collapsible').onclick = function () {
        var checkCollapse = false;

        if (checkCollapse == false) {
            var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
            for (i = 0; i < schemaData.length; i++) {
                document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = "inherit";
                checkCollapse = true;
            }
        }
        else if (checkCollapse) {
            var schemaData = document.getElementsByClassName('collapsible-govuk-grid');
            for (i = 0; i < schemaData.length; i++) {
                document.getElementsByClassName('collapsible-govuk-grid')[i].style.display = 'none';
                checkCollapse = false;
            }
        }
    }
}