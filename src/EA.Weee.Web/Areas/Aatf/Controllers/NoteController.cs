namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;
    using Api.Client;
    using EA.Weee.Web.Areas.AatfEvidence.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfEvidence.ViewModels;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
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
                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));

                var model = mapper.Map<CreateNoteViewModel>(new CreateNoteMapTransfer(schemes));

                await SetBreadcrumb(organisationId, "TODO:fix");

                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}