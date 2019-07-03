function showWarning(show) {

    var warningText = $("#warning-text");

    if (show == true) {
        warningText.removeClass('hidden');
    } else {
        warningText.addClass('hidden');
    }
}