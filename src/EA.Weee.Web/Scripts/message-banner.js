$(async function () {
    try {
        const url = buildBannerUrl();
        const result = await $.get(url);

        const $banner = $('#dvMessageBanner');

        if (result?.IsActive) {
            $('#title').html(result.Title);
            $('#spDescription').html(result.Description);
            $banner.show();
        } else {
            $banner.hide();
        }
    } catch (err) {
        console.error('Failed to load message banner', err);
    }
});

function buildBannerUrl() {
    const { protocol, host, pathname } = location;
    const segments = pathname.split('/').filter(Boolean);

    // If UAT, include the app segment
    const basePath = host.includes('uat') && segments.length ? `/${segments[0]}` : '';

    return `${protocol}//${host}${basePath}/Banner/MessageBannerAsync`;
}
