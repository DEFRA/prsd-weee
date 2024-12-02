(() => {
    window.orgDetailsCompanyLookup = function (endpoint) {
        let findButtonId = "find-company";

        let config = {
            inputFieldId: "CompaniesRegistrationNumber",
            endpoint: endpoint,
            findButtonId: findButtonId,     
            onRetreived: (response, input) => {
                let errorClass = "govuk-form-group--error";
                
                let $parent = $("#" + findButtonId).parent().parent();

                let $errorBanner = $(".companies-house-search-error");

                if (!response.LookupFound) {
                    $errorBanner.show();
                    $parent.addClass(errorClass)
                } else {
                    $errorBanner.hide();
                    $parent.removeClass(errorClass)
                }

                if (!response.LookupFound) return;

                setAddress(response.Address);
                
                $("#CompanyName").val(response.CompanyName);
            }
        }

        function setAddress(address) {
            $("#Address_Address1").val(address.Address1);
            $("#Address_Address2").val(address.Address2);
            $("#Address_TownOrCity").val(address.TownOrCity);
            $("#Address_CountyOrRegion").val(address.CountyOrRegion);
            $("#Address_Postcode").val(address.Postcode);

            setCountry(address)
        }

        function setCountry(selected) {
            let $dropdownParent = $("#operator-country-dropdown-list");
            let $select = $dropdownParent.find("select");

            $select.find(`option[value='${selected.CountryId}']`).prop('selected', true);
            $select.val(selected.CountryId);
            $dropdownParent.find("input").val($select.find("option:selected").text());
        }

        $(document).ready(() => window.lookupSearch(config));
    }
})();