using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.Prsd.Core.Mapper;
using EA.Weee.Api.Client;
using EA.Weee.Web.Areas.Admin.Controllers;
using EA.Weee.Web.Areas.Admin.Controllers.Base;
using EA.Weee.Web.Services;
using EA.Weee.Web.Services.Caching;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    public class OrganisationControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly OrganisationController controller;

        public OrganisationControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new OrganisationController(() => apiClient, cache, breadcrumb, mapper);
        }

        [Fact]
        public void OrganisationControllerInheritsAdminController()
        {
            typeof(OrganisationController).BaseType.Name.Should().Be(typeof(AdminController).Name);
        }
    }
}
