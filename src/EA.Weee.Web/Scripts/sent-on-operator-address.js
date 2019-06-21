function IsOperatorSameAddress() {
    var checkbox = $('#IsOperatorTheSameAsAATF')[0];
    if (checkbox.checked) {
        var address1 = $('#SiteAddressData_Address1').val();
        var address2 = $('#SiteAddressData_Address2').val();
        var townorcity = $('#SiteAddressData_TownOrCity').val();
        var postcode = $('#SiteAddressData_Postcode').val();
        var countyorregion = $('#SiteAddressData_CountyOrRegion').val();
        var countryId = $('#SiteAddressData_CountryId').val();
        var countryName = $('#SiteAddressData_CountryName').val();

        if (address1 != '') {
            $('#OperatorAddressData_Address1').val(address1);
        }

        if (address2 != '') {
            $('#OperatorAddressData_Address2').val(address2);
        }

        if (townorcity != '') {
            $('#OperatorAddressData_TownOrCity').val(townorcity);
        }

        if (postcode != '') {
            $('#OperatorAddressData_Postcode').val(postcode);
        }

        if (countyorregion != '') {
            $('#OperatorAddressData_CountyOrRegion').val(countyorregion);
        }

        if (countryId != '') {
            $('#OperatorAddressData_CountryId').val(countryId);
            $('#country-dropdown-list input').val(countryName);
        }
    } else {
        $('#OperatorAddressData_Address1').val('');
        $('#OperatorAddressData_Address2').val('');
        $('#OperatorAddressData_CountyOrRegion').val('');
        $('#OperatorAddressData_Postcode').val('');
        $('#OperatorAddressData_TownOrCity').val('');
        $('#OperatorAddressData_CountryId').val('');
        $('#country-dropdown-list input').val('');
    }
}