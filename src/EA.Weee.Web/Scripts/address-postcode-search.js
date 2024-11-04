(() => {
    window.searchAutocomplete = (config) => {
        let $select, $findButton;

        let init = () => {
            $findButton = getById(config.findButtonId);
            $select = getById(config.selectFieldId);

            checkHasValue();

            addOption("SelectMessage", config.selectMessage, true);

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

            $.get(config.endpoint + inputValue, (res) => {
                let results = res[config.mappedFields.output];

                $select.off('change');
                $select.empty();

                $select.change(function () {
                    let id = $(this).val();

                    let selected = results.find(x => x[config.mappedFields.key] == id);

                    config.onSelected(selected);
                });

                if (config.onRetreived) config.onRetreived(res, inputValue);

                $findButton.prop("disabled", false);

                if (results.length > 0) {
                    results.forEach((item) => addOption(item[config.mappedFields.key], item[config.mappedFields.value]));

                    $select.find("option:odd").addClass('autocomplete__option--odd');
                }
            });
        };

        function checkHasValue() {
            let inputValue = getById(config.inputFieldId).val();

            $findButton.prop("disabled", !inputValue);
        }

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

    window.contactDetailsPostCodeSearch = (endpoint) => {
        let config = {
            inputFieldId: "AddressData_Postcode",
            selectFieldId: "address-results",
            endpoint: endpoint,
            findButtonId: "find-address",
            mappedFields: {
                key: "Uprn",
                value: "AddressLine",
                output: "Results"
            },
            selectMessage: "Search for a postcode",
            onSelected: (selected) => {
                let address = buildLineOneAndTwo(selected);

                $("#AddressData_Address1").val(address.one);
                $("#AddressData_Address2").val(address.two);
                $("#AddressData_TownOrCity").val(selected.Town);
                $("#AddressData_CountyOrRegion").val(selected.AdministrativeArea || selected.HistoricCounty || selected.CeremonialCounty);
                $("#AddressData_Postcode").val(selected.Postcode);
            },
            onRetreived: (response, input) => {
                let $banner = $(".govuk-warning-text");

                let setWarning = (message) => {
                    $banner.show();
                    $banner.find("#text").html(message);
                };

                let setDefaultOption = () => this.addOption("Select", "Search for a postcode", true);

                if (response.InvalidRequest) {
                    setDefaultOption();
                    setWarning(`${input} is an invalid postcode`);
                    return;
                }

                if (response.Results.length < 1) {
                    setDefaultOption();
                    setWarning(`We could not find an address that matches ${input}. You can search again or enter the address manually`)
                } else {
                    $banner.hide();
                    this.addOption("Select", `${response.Results.length} addresses found for ${input}`, true);
                }
            }
        }

        function buildLineOneAndTwo(selected) {
            let one = orEmpty((orEmpty(selected.BuildingNumber) || (orEmpty(selected.SubBuildingName)) + " " + (orEmpty(selected.BuildingName))))

            if (selected.BuildingName && !one.includes(selected.BuildingName)) {
                one = selected.BuildingName + " " + one;
            }

            if (selected.SubBuildingName && !one.includes(selected.SubBuildingName)) {
                one = selected.SubBuildingName + " " + one;
            }

            let showStreet = (!selected.BuildingName && !selected.SubBuildingName) || (!selected.BuildingNumber && !selected.SubBuildingName);

            if (showStreet) {
                one = (one + " " + selected.Street)
            }

            let two = (!showStreet)
                ? selected.Street
                : "";

            return { one: one, two: two };
        }

        function orEmpty(val) {
            return val || "";
        }

        $(document).ready(() => window.searchAutocomplete(config));
    }
})();