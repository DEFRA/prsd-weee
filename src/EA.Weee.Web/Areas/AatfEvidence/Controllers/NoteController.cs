namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using System;
    using System.Dynamic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;

    public class NoteController : AatfEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public NoteController(IMapper mapper, BreadcrumbService breadcrumb, IWeeeCache cache, Func<IWeeeClient> apiClient)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
        }

        public async Task<ActionResult> Create(Guid organisationId)
        {
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));
            }

            await SetBreadcrumb(organisationId, "TODO:fix");

            return View(new CreateNoteViewModel());
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}