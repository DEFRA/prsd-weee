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

                if (response.HasError) {
                    $errorBanner.show();
                    $parent.addClass(errorClass)
                } else {
                    $errorBanner.hide();
                    $parent.removeClass(errorClass)
                }

                if (response.Organisation) {
                    $("#CompanyName").val(response.Organisation.Name)
                }
            }
        }

        $(document).ready(() => window.lookupSearch(config));
    }
})();


