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
        public static async Task BindCountriesList(this Controller controller,
          Func<IWeeeClient> apiClient, IPrincipal user)
        {
            using (var client = apiClient())
            {
                await controller.BindCountriesList(client, user);
            }
        }

        public static async Task BindCountriesList(this Controller controller, IWeeeClient client,
                   IPrincipal user)
        {
            var regions = await client.SendAsync(user.GetAccessToken(), new GetCountries());
            controller.ViewBag.Countries = new SelectList(regions, "Id", "Name");
        }

        public static async Task BindUKRegionsList(this Controller controller,
            Func<IWeeeClient> apiClient, IPrincipal user)
        {
            using (var client = apiClient())
            {
                await controller.BindUKRegionsList(client, user);
            }
        }

        public static async Task BindUKRegionsList(this Controller controller, IWeeeClient client,
            IPrincipal user)
        {
            var ukcountryData = await client.SendAsync(user.GetAccessToken(), new GetUKCompetentAuthorities());
            var countries = await client.SendAsync(user.GetAccessToken(), new GetCountries());
            
            var ukregions = countries.Join(ukcountryData, c => c.Id, u => u.CountryId, (c, u) => new {c.Id, c.Name});
            controller.ViewBag.Countries = new SelectList(ukregions, "Id", "Name");
        }

        public static string GetCountrybyId(this Controller controller, Guid? id)
        {
            if (id != null)
            {
                var countries = (SelectList)controller.ViewBag.Countries;
                var country = countries.SingleOrDefault(r => r.Value == id.ToString());
                if (country != null)
                {
                    return country.Text;
                }
            }
            return string.Empty;
        }
    }
}