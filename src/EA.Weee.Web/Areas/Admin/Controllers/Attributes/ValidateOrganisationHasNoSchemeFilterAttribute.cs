namespace EA.Weee.Web.Areas.Admin.Controllers.Attributes
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ValidateOrganisationHasNoSchemeFilterAttribute : ValidateOrganisationSchemeBaseActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid organisationId)
        {
            using (var client = Client())
            {
                SchemeData schemeData = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetSchemeByOrganisationId(organisationId));

                if (schemeData != null)
                {
                    filterContext.Result = new RedirectResult("~/admin/scheme/manage-schemes");
                }
            }
        }
    }
}