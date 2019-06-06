function showWarning(show) {

    var warningText = $("#yes-warning");

    if (show == true) {
        warningText.removeClass('hidden');
    } else {
        warningText.addClass('hidden');
    }
}