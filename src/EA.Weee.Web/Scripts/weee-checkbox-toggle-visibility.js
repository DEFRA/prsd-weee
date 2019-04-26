function toggleVisibility(toggle, section) {
    $(document).ready(function () {
        $(section).css('display', 'none');
        $(toggle).change(function () {
            if (this.checked) {
                console.log("Toggled - On")
                $(section).css('display', 'block');
            } else {
                $(section).css('display', 'none');
            }
        });
    });
}