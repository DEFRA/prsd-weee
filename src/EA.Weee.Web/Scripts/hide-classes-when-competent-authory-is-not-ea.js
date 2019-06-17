$(document).ready(function () {
    var competentyAuthority = $("#CompetentAuthorityId option:selected").html();

    if (competentyAuthority == "EA") {
        $(".competent-authority-hidden").css("display", "block");
    }
    else {
        $(".competent-authority-hidden").css("display", "none");
    }
});

$("#CompetentAuthorityId").change(function () {
    var competentyAuthority = $("#CompetentAuthorityId option:selected").html();

    if (competentyAuthority == "EA") {
        $(".competent-authority-hidden").css("display", "block");
    }
    else {
        $(".competent-authority-hidden").css("display", "none");
    }
});