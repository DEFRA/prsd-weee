namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Search;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class AddPcsController : AdminController
    {
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly int maximumSearchResults;
        private readonly IFacilityViewModelBaseValidatorWrapper validationWrapper;

        public AddPcsController(
            ISearcher<OrganisationSearchResult> organisationSearcher,
            Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            ConfigurationService configurationService,
            IFacilityViewModelBaseValidatorWrapper validationWrapper)
        {
            this.organisationSearcher = organisationSearcher;
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.validationWrapper = validationWrapper;

            maximumSearchResults = configurationService.CurrentConfiguration.MaximumAatfOrganisationSearchResults;
        }
    }
}