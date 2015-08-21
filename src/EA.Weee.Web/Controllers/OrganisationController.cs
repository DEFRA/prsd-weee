namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Organisations;
    using Core.Shared;
    using Infrastructure;
    using ViewModels.Organisation;
    using ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Users;

    public class OrganisationController : ExternalSiteController
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
                     new GetUserOrganisationsByStatus(new[] { (int)UserStatus.Pending, (int)UserStatus.Refused, (int)UserStatus.Inactive }));

                model.OrganisationUserData = pendingOrganisationUsers;
            }

            return View("HoldingMessageForPending", model);
        }
    }
}