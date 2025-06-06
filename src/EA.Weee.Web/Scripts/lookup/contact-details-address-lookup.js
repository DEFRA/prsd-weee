﻿(() => {
    window.contactDetailsAddressLookup = (endpoint) => {
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
                $("#AddressData_CountyOrRegion").val(selected.CeremonialCounty || selected.AdministrativeArea || selected.HistoricCounty);
                $("#AddressData_Postcode").val(selected.Postcode);

                setCountry(selected);
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

        function setCountry(selected) {
            let $dropdownParent = $("#operator-country-dropdown-list");
            let $select = $dropdownParent.find("select");

            $select.find(`option[value='${selected.CountryId}']`).prop('selected', true);
            $select.val(selected.CountryId);
            $dropdownParent.find("input").val($select.find("option:selected").text());
        }

        function orEmpty(val) {
            return val || "";
        }

        $(document).ready(() => window.selectLookupSearch(config));
    }
})();


