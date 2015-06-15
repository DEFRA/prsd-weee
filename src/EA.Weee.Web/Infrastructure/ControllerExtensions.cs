namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Weee.Requests.Shared;

    public static class ControllerExtensions
    {
        public static async Task BindUKCompetentAuthorityRegionsList(this Controller controller,
            Func<IWeeeClient> apiClient)
        {
            using (var client = apiClient())
            {
                await controller.BindUKCompetentAuthorityRegionsList(client);
            }
        }

        public static async Task BindUKCompetentAuthorityRegionsList(this Controller controller, IWeeeClient apiClient)
        {
            var regions = await apiClient.SendAsync(new GetUKCompetentAuthorities());
            controller.ViewBag.UKRegions = new SelectList(regions, "Id", "Region");
        }
    }
}