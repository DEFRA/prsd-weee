namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Helper;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using ViewModels.AddOrganisation;
    using ViewModels.AddOrganisation.Details;
    using ViewModels.AddOrganisation.Type;

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