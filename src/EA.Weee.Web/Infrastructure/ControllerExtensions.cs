namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Weee.Requests.Shared;

    public static class ControllerExtensions
    {
        public static async Task BindUKCompetentAuthorityRegionsList(this Controller controller,
            Func<IWeeeClient> apiClient, IPrincipal user)
        {
            using (var client = apiClient())
            {
                await controller.BindUKCompetentAuthorityRegionsList(client, user);
            }
        }

        public static async Task BindUKCompetentAuthorityRegionsList(this Controller controller, IWeeeClient client,
            IPrincipal user)
        {
            var regions = await client.SendAsync(user.GetAccessToken(), new GetUKCompetentAuthorities());
            controller.ViewBag.UKRegions = new SelectList(regions, "Id", "Region");
        }

        public static string GetUKRegionById(this Controller controller, Guid? id)
        {
            if (id != null)
            {
                var regions = (SelectList)controller.ViewBag.UKRegions;
                var region = regions.SingleOrDefault(r => r.Value == id.ToString());
                if (region != null)
                {
                    return region.Text;
                }
            }
            return string.Empty;
        }

        public static Guid GetDefaultUKCountryId(this Controller controller)
        {
            var regions = (SelectList)controller.ViewBag.UKRegions;

            var defaultId = regions.Single(c => c.Text.Equals("England", StringComparison.InvariantCultureIgnoreCase)).Value;

            return new Guid(defaultId);
        }
    }
}