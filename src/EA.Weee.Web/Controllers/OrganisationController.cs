namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using ViewModels.Organisation;
    using Weee.Requests.Organisations;

    public class OrganisationController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public OrganisationController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> HoldingMessageForPending()
        {
            var model = new OrganisationUserPendingViewModel();

            using (var client = apiClient())
            {
                var pendingOrganisationUsers = await
                 client.SendAsync(
                     User.GetAccessToken(),
                     new GetOrganisationsByUserId(User.GetUserId(), new[] { (int)OrganisationUserStatus.Pending, (int)OrganisationUserStatus.Refused, (int)OrganisationUserStatus.Inactive }));

                model.OrganisationUserData = pendingOrganisationUsers;
            }

            return View("HoldingMessageForPending", model);
        }
    }
}