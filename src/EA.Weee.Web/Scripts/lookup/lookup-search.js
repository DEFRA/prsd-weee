(() => {
    window.lookupSearch = (config) => {
        let $findButton, $spinner;

        let init = () => {
            $findButton = getById(config.findButtonId);

            $spinner = $("#spinner");

            checkHasValue();

            getById(config.inputFieldId).on("keyup change paste", checkHasValue);

            getById(config.inputFieldId).on("keypress", (event) => {
                if (event.which != 13) return;

                event.preventDefault();

                $findButton.click();
                $findButton.focus();
            });

            $findButton.click(search);
        };

        let search = () => {
            let inputValue = getById(config.inputFieldId).val();

            $findButton.prop("disabled", true);

            $spinner.show();

            $.get(config.endpoint + inputValue, (res) => {
                $spinner.hide();

                if (config.onRetreived) config.onRetreived(res, inputValue);

                $findButton.prop("disabled", false);
            });
        };

        function checkHasValue() {
            let inputValue = getById(config.inputFieldId).val();

            $findButton.prop("disabled", !inputValue);
        }

        function getById(id) { return $('#' + id); };

        init();
    }

    window.selectLookupSearch = (config) => {
        let $select;

        let innerConfig = {
            inputFieldId: config.inputFieldId,
            endpoint: config.endpoint,
            findButtonId: config.findButtonId,
            onRetreived: (response, input) => {
                let results = response[config.mappedFields.output];

                $select.off('change');
                $select.empty();
                $select.change(function () {
                    let id = $(this).val();

                    let selected = results.find(x => x[config.mappedFields.key] == id);

                    config.onSelected(selected);
                });

                if (config.onRetreived) config.onRetreived(response, input);

                if (results.length > 0) {
                    results.forEach((item) => addOption(item[config.mappedFields.key], item[config.mappedFields.value]));

                    $select.find("option:odd").addClass('autocomplete__option--odd');

                    $select.val("Select");
                }
            }
        }

        let init = () => {
            $select = getById(config.selectFieldId);

            addOption("SelectMessage", config.selectMessage, true);

            window.lookupSearch(innerConfig);
        };

        function getById(id) { return $('#' + id); };

        function addOption(val, html, disabled) {
            let select = $select
                .append($('<option></option>').val(val)
                    .html(html));

            if (disabled === true) {
                $(select).find(`option[value='${val}']`).prop("disabled", "disabled");
            }
        };

        init();

        this.addOption = addOption;
    }
})();