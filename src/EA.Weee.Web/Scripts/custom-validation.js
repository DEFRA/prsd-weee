jQuery.validator.setDefaults({
    onfocusout: false,
    onkeyup: false,
    onclick: false,
    focusInvalid: false,
    errorClass: "",
    // Create a custom error summary that will appear before the form
    showErrors: function (errorMap, errorList) {
        if (errorList.length !== 0) {

            var headerErrorCount = "";

            if (errorList.length === 1) {
                headerErrorCount = " 1 error";
            }
            else {
                headerErrorCount = errorList.length + " errors";
            }

            summary = "<h2 class='error-summary-heading heading-medium govuk-error-summary__title' id=\"error-summary-title\">You have " + headerErrorCount + " on this page</h2>";
            summary += '<div class="govuk-error-summary__body">';
            summary += '<ul class="govuk-list govuk-error-summary__list"';

            for (error in errorList) {
                summary += '<li><a href="#' + errorList[error].element.id + '">' + errorList[error].message + '</a></li>';

                $(errorList[error].element).parents(".govuk-form-group").last().addClass("govuk-form-group--error");
            }

            summary += "</ul>";
            summary += "</div>";
            //// Toggle classes on validation summary
            $('.error-summary-valid').removeClass('validation-summary-errors');
            $('.error-summary-valid').removeClass('error-summary');
            $('.error-summary-valid').addClass('govuk-error-summary');
            $('.error-summary-valid').attr('data-module', 'govuk-error-summary');
            //// Output our error summary and place it in the error container
            $('.error-summary-valid').html(summary);
            
            //$(".input-validation-error").removeClass("govuk-file-upload input-validation-errror");
            //// Focus on first error link in the error container 
            $('.error-summary-valid').first().focus();
        }
        this.defaultShowErrors();
    }
});