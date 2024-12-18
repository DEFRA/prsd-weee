﻿function SearchAnAATF() {
    $("#SearchTerm").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "SearchAnAatf/SearchAatf",
                type: "POST",
                dataType: "json",
                data: { searchTerm: request.term, currentSelectedAatfId: request.AatfId, currentSelectedReturnId: request.ReturnId, __RequestVerificationToken: $("[name=__RequestVerificationToken]").val() },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.SearchTermName, value: item.SearchTermName, id: item.SearchTermId };
                    }))
                }
            })
        },
        minLength: 1,
        select: function (event, ui) {
            $("#hdnSelectedAatfId").val(ui.item.id);
        },
        messages: {
            noResults: "", results: ""
        }
    });
}