function toggleVisibility(toggle, section) {
    $(document).ready(function () {
        var attr = $(toggle).attr('checked');
        console.log(attr);
        if (!$('#WeeeReusedOptions.govuk-form-group').hasClass('dcf-section-error')) {
            if (!(attr !== undefined)) {
                $(section).css('display', 'none');
            }
        }
        
        $(toggle).change(function () {
            if (this.checked) {
                $(section).css('display', 'block');
            } else {
                $(section).css('display', 'none');
            }
        });
    });
}