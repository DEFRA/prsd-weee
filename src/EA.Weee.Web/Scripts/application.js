function ShowHideContent() {
    var self = this;


    self.escapeElementName = function (str) {
        result = str.replace('[', '\\[').replace(']', '\\]')
        return (result);
    };

    self.showHideRadioToggledContent = function () {
        $(".block-label input[type='radio']").each(function () {

            var $radio = $(this);
            var $radioGroupName = $radio.attr('name');
            var $radioLabel = $radio.parent('label');

            var dataTarget = $radioLabel.attr('data-target');

            // Add ARIA attributes

            // If the data-target attribute is defined
            if (dataTarget) {

                // Set aria-controls
                $radio.attr('aria-controls', dataTarget);

                $radio.on('click', function () {

                    // Select radio buttons in the same group
                    $radio.closest('form').find(".block-label input[name='" + self.escapeElementName($radioGroupName) + "']").each(function () {
                        var $this = $(this);

                        var groupDataTarget = $this.parent('label').attr('data-target');
                        var $groupDataTarget = $('#' + groupDataTarget);

                        // Hide toggled content
                        $groupDataTarget.hide();
                        // Set aria-expanded and aria-hidden for hidden content
                        $this.attr('aria-expanded', 'false');
                        $groupDataTarget.attr('aria-hidden', 'true');
                    });

                    var $dataTarget = $('#' + dataTarget);
                    $dataTarget.show();
                    // Set aria-expanded and aria-hidden for clicked radio
                    $radio.attr('aria-expanded', 'true');
                    $dataTarget.attr('aria-hidden', 'false');

                });

            } else {
                // If the data-target attribute is undefined for a radio button,
                // hide visible data-target content for radio buttons in the same group

                $radio.on('click', function () {

                    // Select radio buttons in the same group
                    $(".block-label input[name='" + self.escapeElementName($radioGroupName) + "']").each(function () {

                        var groupDataTarget = $(this).parent('label').attr('data-target');
                        var $groupDataTarget = $('#' + groupDataTarget);

                        // Hide toggled content
                        $groupDataTarget.hide();
                        // Set aria-expanded and aria-hidden for hidden content
                        $(this).attr('aria-expanded', 'false');
                        $groupDataTarget.attr('aria-hidden', 'true');
                    });

                });
            }

        });
    }
    self.showHideCheckboxToggledContent = function () {

        $(".block-label input[type='checkbox']").each(function () {

            var $checkbox = $(this);
            var $checkboxLabel = $(this).parent();

            var $dataTarget = $checkboxLabel.attr('data-target');

            // Add ARIA attributes

            // If the data-target attribute is defined
            if (typeof $dataTarget !== 'undefined' && $dataTarget !== false) {

                // Set aria-controls
                $checkbox.attr('aria-controls', $dataTarget);

                // Set aria-expanded and aria-hidden
                $checkbox.attr('aria-expanded', 'false');
                $('#' + $dataTarget).attr('aria-hidden', 'true');

                // For checkboxes revealing hidden content
                $checkbox.on('click', function () {

                    var state = $(this).attr('aria-expanded') === 'false' ? true : false;

                    // Toggle hidden content
                    $('#' + $dataTarget).toggle();

                    // Update aria-expanded and aria-hidden attributes
                    $(this).attr('aria-expanded', state);
                    $('#' + $dataTarget).attr('aria-hidden', !state);

                });
            }

        });
    }
}

$(document).ready(function () {

    // Focus on first validation error input, when validation errors occur
    if ($("error_explanation") != null) {
        $(".input-validation-error:input:first").focus();
    }

    // Turn off jQuery animation
    jQuery.fx.off = true;

    // Use GOV.UK selection-buttons.js to set selected
    // and focused states for block labels
    var $blockLabels = $(".block-label input[type='radio'], .block-label input[type='checkbox']");
    new GOVUK.SelectionButtons($blockLabels);

    // Details/summary polyfill
    // See /javascripts/vendor/details.polyfill.js

    // Where .block-label uses the data-target attribute
    // to toggle hidden content
    var toggleContent = new ShowHideContent();
    toggleContent.showHideRadioToggledContent();
    toggleContent.showHideCheckboxToggledContent();

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
    var countryInput = $(".form-group.countries select");
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