namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class SentOnRemoveSiteControllerTests
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly SentOnRemoveSiteController controller;
        private readonly IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel> mapper;

        public SentOnRemoveSiteControllerTests()
        {
            this.apiClient = A.Fake<Func<IWeeeClient>>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel>>();

               controller = new SentOnRemoveSiteController(apiClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void CheckSentOnCreateSiteOperatorControllerInheritsExternalSiteController()
        {
            typeof(SentOnRemoveSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }
    }
}
