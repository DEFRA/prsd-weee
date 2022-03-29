$(document).ready(function () {
    function competentAuthoritySelection() {
	    var competentAuthority = $("#CompetentAuthorityId option:selected").html();

        if (competentAuthority === "EA") {
            $(".competent-authority-hidden").css("display", "block");

            $('a[href*="PanAreaId"]').parent().css("display", "");
            $('a[href*="LocalAreaId"]').parent().css("display", ""); 

            $('.error-summary-list').parent().css("display", "block");

            $('#error-summary-title').text("There is a problem");
	    }
        else
        {
            $(".competent-authority-hidden").css("display", "none");
            $(".competent-authority-hidden").find("select").val('');

            $('a[href*="PanAreaId"]').parent().css("display", "none");
            $('a[href*="LocalAreaId"]').parent().css("display", "none");

            var numberOfNonHiddenErrors = $('.error-summary-list li').not('[style]').length;
            if (!numberOfNonHiddenErrors > 0) {
	            $('.error-summary-list').parent().css("display", "none");
            } else {
	            $('#error-summary-title').text("There is a problem");
            }
            
        }
    }

    competentAuthoritySelection();

    $("#CompetentAuthorityId").change(function () {
	    competentAuthoritySelection();
    });
});

