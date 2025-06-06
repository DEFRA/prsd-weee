﻿$(document).ready(function () {

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
    $('input[type=submit], button').bind('click', function () {
        return disableButtonFor1Second($(this));
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
    countryInput.each(function () {
        $(this).next("input").attr("autocomplete", "new-password");

        if ($(this).hasClass("input-validation-error")) {
            var validationInput = $(this).next("input");
            var id = $(this).attr("id");
            $(this).removeAttr("id");
            validationInput.attr("id", id);
        }
    });

    $(window).resize(function () {

        function positionDiv(input, div) {
            var inputTop = input.offset().top;
            var inputHeight = input.outerHeight();
            var divTop = inputTop + inputHeight;

            $(div).css({
                position: "absolute",
                top: divTop,
                left: input.offset().left
            });
        }

        //fix the search and country autocompletes
        var autocompleteInput = $("#SearchTerm");
        var autoCompleteSearchDiv = $(".ui-autocomplete");

        if (autocompleteInput.length > 0) {
            positionDiv(autocompleteInput, autoCompleteSearchDiv);
        }

        var countryInput = $(".govuk-form-group.countries select");
        if (countryInput.length > 0) {
            positionDiv(countryInput.next("input"), autoCompleteSearchDiv);
        }
    });

    //fn setCurPosition
    $.fn.setCurPosition = function (pos) {
        this.focus();
        this.each(function (index, elem) {
            if (elem.setSelectionRange) {
                elem.setSelectionRange(pos, pos);
            } else if (elem.createTextRange) {
                var range = elem.createTextRange();
                range.collapse(true);
                range.moveEnd('character', pos);
                range.moveStart('character', pos);
                range.select();
            }
        });
        return this;
    };




    // When a link is clicked in the validation summary, move the focus to the associated input.
    // The link will only set the target, not the focus, to the specified anchor.
    $('.error-summary a').click(function () {
        $($(this).attr('href'));
        var input = $($(this).attr('href'));
        $(input).setCurPosition(0);

        return false;
    });

    $(".date-picker").flatpickr({
        enableTime: false,
        allowInput: true,
        dateFormat: "d/m/Y",
        altFormat: "d/m/Y",
        disableMobile: "true",
        locale: "en"
    }
    );

    function getUrlParameter(name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    }

    var parameter = getUrlParameter("clickedTab");
    if (parameter !== "") {
        $("#" + parameter).focus();
    }

    $("#ReturnsExternalSelectedComplianceYear").change(function () {
        $("#ReturnsQueryParameterForm").submit();
    });
    $("#ReturnsExternalSelectedQuarter").change(function () {
        $("#ReturnsQueryParameterForm").submit();
    });
    $("#CopyPreviousSchemes").change(function () {
        $("#SelectPcsForm").submit();
    });

    $("#TransferAllTonnage").change(function () {
        $("#TransferEvidenceForm").submit();
    });
    $("#SelectedComplianceYear").change(function () {
        $("#ComplianceYearForm").submit();
    });

    setupAutoCompletes();
});

function setupAutoCompletes() {

    var autoCompleteEvent = new CustomEvent("gds-auto-complete-event", {
        detail: {},
        bubbles: true,
        cancelable: true,
        composed: false
    });

    var selectElements = document.querySelectorAll(".gds-auto-complete");

    selectElements.forEach(function (element) {
        var items = Array.from(element.options).map(el => el.textContent || el.innerText);

        // get the default selected value
        var selected = null;
        for (var optionsCount = 0; optionsCount < element.options.length; optionsCount++) {
            var opt = element.options[optionsCount];
            if (opt.selected === true) {
                selected = opt.text;
                break;
            }
        }
        var useOverLay = element.classList.contains("use-overlay");
        var newElement = document.createElement("div");
        newElement.setAttribute("style", "width: 100%");

        element.parentNode.insertBefore(newElement, element);

        var existingId = element.id;

        // before creating the new element ensure that an auto complete with the id being created isnt already
        var autocompleteCreated = $(element.parentNode).find(".autocomplete__dropdown-arrow-down-wrapper");

        if (!autocompleteCreated || autocompleteCreated.length === 0) {

            accessibleAutocomplete({
                showAllValues: true,
                id: element.id,
                source: items,
                defaultValue: selected,
                element: newElement,
                displayMenu: useOverLay ? 'overlay' : 'inline',
                name: element.id + "-auto",
                onConfirm: function (confirmed) {

                    function isNullOrWhitespace(input) {
                        if (typeof input === "undefined" || input == null) return true;
                        return input.replace(/\s/g, "").length < 1;
                    }

                    var postBackElement = document.getElementById(existingId + "-select");
                    var selectedValue = document.getElementById(existingId).value;

                    if (!isNullOrWhitespace(confirmed) || !isNullOrWhitespace(selectedValue)) {
                        var selected = confirmed || selectedValue;
                        for (var postBackOptions = 0;
                            postBackOptions < postBackElement.options.length;
                            postBackOptions++) {
                            var findSelectedOption = postBackElement.options[postBackOptions];
                            var text = findSelectedOption.textContent || findSelectedOption.innerText;
                            if (text === selected) {
                                if (postBackElement.value !== findSelectedOption.value) {
                                    postBackElement.value = findSelectedOption.value;
                                    postBackElement.dispatchEvent(autoCompleteEvent);
                                }
                            }
                        }
                    } else {
                        postBackElement.value = null;
                        if (isNullOrWhitespace(selectedValue)) {
                            postBackElement.dispatchEvent(autoCompleteEvent);
                        }
                    }
                },
                dropdownArrow: function (config) {
                    return '<svg class="autocomplete__dropdown-arrow-down" viewBox="0 0 512 512" ><path d="M256,298.3L256,298.3L256,298.3l174.2-167.2c4.3-4.2,11.4-4.1,15.8,0.2l30.6,29.9c4.4,4.3,4.5,11.3,0.2,15.5L264.1,380.9  c-2.2,2.2-5.2,3.2-8.1,3c-3,0.1-5.9-0.9-8.1-3L35.2,176.7c-4.3-4.2-4.2-11.2,0.2-15.5L66,131.3c4.4-4.3,11.5-4.4,15.8-0.2L256,298.3  z"/></svg>'
                }
            });
        }

        if (element.classList.contains("input-validation-error")) {
            var autoCompletes = $(element.parentNode).find(".autocomplete__input");
            autoCompletes[0].classList.add("autocomplete__error");
        }

        var newListBox = document.getElementById(element.id + "__listbox");
        newListBox.setAttribute("aria-labelledby", element.id + "-label");
        var autocomplete = document.getElementById(element.id);
        autocomplete.setAttribute("type", "search");
        autocomplete.removeAttribute("role");
        element.style.display = "none";
        element.id = element.id + "-select";

    });
}
function initReviewEvidenceNote() {
    if ($("#ReasonDiv").hasClass("error")) {
        document.getElementById("ReasonDiv").style = "display: block;";
    }
    else {
        document.getElementById("ReasonDiv").style = "display: none;";
    }
    document.getElementById("conditional-SelectedValue-0").style = "display: block;";
    document.getElementById("conditional-SelectedValue-1").style = "display: block;";
    document.getElementById("conditional-SelectedValue-2").style = "display: block;";

    function showReasonText(event) {
        document.getElementById("ReasonDiv").style = "display: block;";
    }

    function hideReasonText(event) {
        document.getElementById("ReasonDiv").style = "display: none;";
    }

    document.getElementById("SelectedValue-0").addEventListener("click", hideReasonText);
    document.getElementById("SelectedValue-1").addEventListener("click", showReasonText);
    document.getElementById("SelectedValue-2").addEventListener("click", showReasonText);
}

function selectAllTransferNoteJourney() {
    $('#SelectAllCheckboxesSection').css("display", "block");

    // case when selectAll is ticked => all checkboxes get selected, if selectAll is unticked => the other 14 checboxes will be unticked
    $('#all_checkbox_select_id').click(function () {
        $(".govuk-checkboxes__input").prop('checked', $(this).prop('checked'));
    });

    // case when if any of the 14th checkboxes is unticked => untick the selectAll too
    $('.allCategoryCheckBox').change(function () {
        var $this = $(this);

        if ($this.is(":checked")) {
        }
        else {
            if ($('#all_checkbox_select_id').is(":checked")) {
                $('#all_checkbox_select_id').prop("checked", false);
            }
        }
    });

    // case when if all 14th checkboxes are ticked => selectAll gets ticked too
    $('.allCategoryCheckBox').change(function () {
        setDefaultAsChecked();
    });

    function setDefaultAsChecked() {
        var a = $("input[type='checkbox'].allCategoryCheckBox");
        if (a.length === a.filter(":checked").length) {
            $('#all_checkbox_select_id').prop("checked", true);
        }
    }

    setDefaultAsChecked();
};

function setupSelectedYear(url) {
    $("#SelectedYear").change(function () {
        var year = $(this).val();
        if (year) {
            window.location.href = url + '?selectedYear=' + year;
        }
    });
}

function setAAAutoCompleteZIndex() {
    var autoCompletes = document.getElementsByClassName('autocomplete__wrapper');
    autoCompletes[0].style.zIndex = 10;
    autoCompletes[1].style.zIndex = 9;
}

$(".transfer-choose-notes-submit").closest('form').on('submit', function (event) {
    event.preventDefault();
    $(".transfer-choose-notes-submit").prop("disabled", true);
    this.submit();
});

/* When a uer selects an item from the autocomplete, the ID is stored in the hidden input
            called "SelectedOrganisationId".If the user changes the value of the search term
            after making a selected, we need to clear this hidden value.

            To do this, we cannot use the change event because FireFox fires this event
            after the autocomplete's select event completes. This clears the value immediately
            after it is set.

            To avoid this, we can store the original search term in a variable and use the focus
            and blur events to check for a change.The variable is updated by the autocomplete
            select event to avoid the value being cleared after a selection is made.
        */


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


function initJQueryAutoComplete(searchUrl, mapFunction, renderFunction, selectedValueControl, additionalData = {}) {
    var searchTerm = '';
    $("#SearchTerm")
        .focus(function () { searchTerm = $(this).val(); })
        .blur(function () { if (searchTerm != $(this).val()) { selectedValueControl.val(""); } })
        .autocomplete({
            source: function (request, response) {
                var data = {
                    SearchTerm: request.term,
                    __RequestVerificationToken: $("[name=__RequestVerificationToken]").val(),
                    ...additionalData  // Spread the additional data here
                };
                $.ajax({
                    type: "POST",
                    url: searchUrl,
                    context: document.body,
                    data: data,
                    success: (data) => {
                        var data = $.map(data, mapFunction);
                        response(data);
                    }
                });
            },
            minLength: 1,
            delay: 500,
            select: function (event, ui) {
                $(this).val(ui.item.label);
                selectedValueControl.val(ui.item.id);
                searchTerm = $(this).val();

                return false;
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // Do nothing.
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            $(ul).addClass("govuk-body govuk-list govuk-list--bullet");
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append(() => {
                    if (renderFunction) {
                        return renderFunction(item);
                    } else {
                        return "<span>" + item.label + "</span>";
                    }
                })
                .appendTo(ul)
        };
}

function initialiseTabs() {
    var keys = { left: 37, right: 39, up: 38, down: 40 };
    var tabListItems = Array.from(document.querySelectorAll('.govuk-tabs__list-item'));

    var onTabKeydown = function (e) {
        var currentTab = e.target.closest('.govuk-tabs__list-item');
        var currentIndex = tabListItems.indexOf(currentTab);
        var nextIndex;

        switch (e.keyCode) {
            case keys.left:
            case keys.up:
                nextIndex = currentIndex - 1;
                break;
            case keys.right:
            case keys.down:
                nextIndex = currentIndex + 1;
                break;
            default:
                return;
        }

        e.preventDefault();

        if (nextIndex >= 0 && nextIndex < tabListItems.length) {
            var nextTab = tabListItems[nextIndex];

            tabListItems.forEach((tab) => {
                var link = tab.querySelector('a');
                link.setAttribute('aria-selected', 'false');
                link.setAttribute('tabindex', '-1');
            });

            var link = nextTab.querySelector('a');
            link.setAttribute('aria-selected', 'true');
            link.setAttribute('tabindex', '0');
            link.click();
        }
    };

    tabListItems.forEach((tab) => {
        tab.addEventListener('keydown', onTabKeydown);
    });
}

(function ($) {
    let $button = $("#discard-button");
    if (!$button) return;

    let $form = $('form');

    let originalData = $form.serialize();

    let disableButton = () => {
        $button.attr("disabled", true);
        $button.css({ "pointer-events": "none" });
    }

    disableButton();

    $form.on('keyup change paste', 'input, select, textarea', () => {
        let currentData = $form.serialize();

        if (currentData == originalData) {
            disableButton();
        } else {
            $button.removeAttr("disabled");
            $button.css({ "pointer-events": "auto" });
        }
    });
})(jQuery);