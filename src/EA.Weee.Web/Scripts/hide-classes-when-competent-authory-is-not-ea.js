$(document).ready(function () {
    function competentAuthoritySelection() {
	    var competentAuthority = $("#CompetentAuthorityId option:selected").html();

        if (competentAuthority === "EA") {
		    $(".competent-authority-hidden").css("display", "block");
	    }
	    else {
            $(".competent-authority-hidden").css("display", "none");
            $(".competent-authority-hidden").find("select").val('');
        }
    }

    competentAuthoritySelection();

    $("#CompetentAuthorityId").change(function () {
	    competentAuthoritySelection();
    });
});

