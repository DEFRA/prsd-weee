function toggleVisibility(toggle, section) {
    $(document).ready(function () {
        if (!$('.govuk-form-group').hasClass('error')) {
            $(section).css('display', 'none');
        }
        $(toggle).change(function () {
            if (this.checked) {
                $(section).css('display', 'block');
            } else {
                $(section).css('display', 'none');
                $('#error_explanation').css('display', 'none');
                $('.govuk-form-group').removeClass('govuk-form-group--error').removeClass('error');
                $('.govuk-radios__input').removeClass('input-validation-error');
            }
        });
    });
}