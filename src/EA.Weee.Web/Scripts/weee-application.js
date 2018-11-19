$(document).ready(function () {

    function focusMainContent() {
        $("#main-content").attr("tabindex", 1);
        $("#main-content").focus(function () {
            $(this).attr("tabindex", 0);
        });
    }

    // When focus moves away from main content container, make main content unfocusable
    $("#main-content").focusout(function () {
        $(this).removeAttr("tabindex");
    });

    // When the skip to content link is clicked, move the focus to the main content.
    $(".skiplink").click(function () {
        focusMainContent();
    });

    // When a validation error exists, move the focus to the the main content
    if ($("#error_explanation").length) {
        focusMainContent();
    }

    // Generic double-click prevention script (used by every button)
    function disableButtonFor1Second(button) {
        if (button.data('disabledCount') == null) button.data('disabledCount', 0)
        var isDisabled = button.data('disabledCount') > 0;
        button.data('disabledCount', button.data('disabledCount') + 1);
        setTimeout(function () {
            button.data('disabledCount', button.data('disabledCount') - 1);
        }, 1000);
        return !isDisabled;
    }

    // Bind the disabling behaviour to all buttons.
    $('input[type=submit], button').bind('click', function() {
        return disableButtonFor1Second($(this));
    });

    // There is a bug with jQuery UI autocomplete whereby the content
    // of the drop-down list becomes detached from the text-field when
    // the window is resized. A simple fix for this is to hide the
    // content if the window is resized. It will be re-positioned the
    // next time it is displayed.
    // See: http://stackoverflow.com/questions/8037483/repositioning-jquery-ui-autocomplete-on-browser-resize
    $(window).resize(function () {
        $('.ui-autocomplete').css('display', 'none');
    });

    // There is a bug with jQuery UI autocomplete whereby the content
    // of the drop-down list has the incorrect width. a fix for this
    // is to override the implementation of "_resizeMenu" to correctly
    // identify the parent element of the list.
    // See: http://stackoverflow.com/questions/5643767/jquery-ui-autocomplete-width-not-set-correctly
    jQuery.ui.autocomplete.prototype._resizeMenu = function () {
        var ul = this.menu.element;
        ul.outerWidth(this.element.outerWidth());
    }

    // Set the country drop-down list for any address to use auto-complete.
    var countryInput = $(".govuk-form-group.countries select");
    countryInput.selectToAutocomplete();

    // When there is a validation erorr, move the ID from the select element to the auto-complete
    // textbox so that the links in the validation summary will work.
    if (countryInput.hasClass("input-validation-error")) {
        var validationInput = countryInput.next("input");
        var id = countryInput.attr("id");
        countryInput.removeAttr("id");
        validationInput.attr("id", id);
    }

    // When a link is clicked in the validation summary, move the focus to the associated input.
    // The link will only set the target, not the focus, to the specified anchor.
    $('.error-summary a').click(function () {
        $($(this).attr('href')).focus();
        return false;
    });
});

//USAGE: $("#form").serializeFiles();
(function ($) {
    $.fn.serializeFiles = function () {
        var obj = $(this);
        /* ADD FILE TO PARAM AJAX */
        var formData = new FormData();
        $.each($(obj).find("input[type='file']"), function (i, tag) {
            $.each($(tag)[0].files, function (i, file) {
                formData.append(tag.name, file);
            });
        });
        var params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
        });
        return formData;
    };
})(jQuery);