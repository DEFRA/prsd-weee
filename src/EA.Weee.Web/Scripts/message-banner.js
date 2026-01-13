$(document).ready(function () {

    getMessageBanner();

    async function getMessageBanner() {

        let url = null;

        if (location.host.includes('uat')) {
            url = location.protocol + '//' + location.host + '/' + location.pathname.split('/')[1] + '/Banner/MessageBannerAsync';
        }
        else {
            url = location.protocol + '//' + location.host + '/Banner/MessageBannerAsync';
        }

        await $.ajax({
            url: url,
            type: "Get",
            data: '',
            contentType: 'application/x-www-form-urlencoded;charset=UTF-8',
            success: function (result) {
                if (result.IsActive) {
                    $('#title').html(result.Title);
                    $('#spDescription').html(result.Description);
                    $('#dvMessageBanner').show();
                }
                else {
                    $('#dvMessageBanner').hide();
                }
            }
        });
    }
});