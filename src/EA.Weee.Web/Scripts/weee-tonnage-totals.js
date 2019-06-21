(function () {

    if (typeof window.CustomEvent === "function") return false;

    function CustomEvent(event, params) {
        params = params || { bubbles: false, cancelable: false, detail: undefined };
        var evt = document.createEvent('CustomEvent');
        evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
        return evt;
    }

    CustomEvent.prototype = window.Event.prototype;

    window.CustomEvent = CustomEvent;
})();

var countDecimals = function (value) {
    if (value.indexOf(".") !== -1) {
        if (Math.floor(value) !== value) {
            return value.toString().split(".")[1].length || 0;
        }
        return 0;
    }
    return 0;
};


function numberWithoutCommas(x) {
    return x.replace(/(\d+),(?=\d{3}(\D|$))/g, "$1");
}

function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function TonnageTotals(controlId) {
    //var controlId = 'Tonnage';
    var tonnageElements = document.querySelectorAll('input[id$=' + controlId + ']');
    for (var elementCount = 0; elementCount < tonnageElements.length; elementCount++) {
        var element = tonnageElements[elementCount];
        element.addEventListener('blur',
            function (e) {
                var event = new CustomEvent("custom-event", {
                    'detail': {
                        controlId: controlId,
                        tonnageElements: tonnageElements
                    }
                });
                this.dispatchEvent(event);
            });
        element.addEventListener('custom-event',
            function (e) {
                var totalId = e.detail.controlId + 'Total';
                var tonnageTotal = document.querySelector('#' + totalId);
                var totalTonnage = 0.00;
                for (var elementCount = 0; elementCount < e.detail.tonnageElements.length; elementCount++) {
                    var element = e.detail.tonnageElements[elementCount];
                    var value = numberWithoutCommas(element.value).trim();

                    if (!isNaN(value) && value && value.indexOf("+") === -1) {
                        var convertedValue = parseFloat(value);
                        if (convertedValue > 0 && countDecimals(value) <= 3) {
                            totalTonnage += parseFloat(convertedValue);
                        }
                    }
                    tonnageTotal.innerText = numberWithCommas(totalTonnage.toFixed(3));
                }
            });
    }
}
