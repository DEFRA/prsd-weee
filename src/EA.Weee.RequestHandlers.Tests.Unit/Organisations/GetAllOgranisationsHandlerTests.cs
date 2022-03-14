namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using Core.Organisations;
    using Domain.Organisation;
    using EA.Weee.DataAccess;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using OrganisationStatus = Core.Shared.OrganisationStatus;

    public class GetAllOgranisationsHandlerTests
    {
        private readonly GetAllOrganisationsHandler handler;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationNameStatus> mapper;
        private readonly DbContextHelper dbHelper;
        private readonly GetAllOrganisations getAllOrganisationQuery;

        public GetAllOgranisationsHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            mapper = A.Fake<IMap<Organisation, OrganisationNameStatus>>();
            handler = new GetAllOrganisationsHandler(context, mapper);
            dbHelper = new DbContextHelper();
            getAllOrganisationQuery = new GetAllOrganisations();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));
        }

        [Fact]
        public async Task HandleAsync_GetAllAvailableOrganisations_ShouldCallTheMapperForEachOrganisationReturned()
        {
            var organisations = CreateOrganisationList();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(organisations));

            await handler.HandleAsync(getAllOrganisationQuery);

            foreach (var org in organisations)
            {
                A.CallTo(() => mapper.Map(org)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        public async Task HandleAsync_GetAllAvailableOrganisations_OrganisationIsMappedToOrganisationNameStatus()
        {
            var organisation = new Organisation();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var mappedOrganisationNameStatus = new OrganisationNameStatus
            {
                Name = organisation.OrganisationName,
                Status = (OrganisationStatus)
                        Enum.Parse(typeof(OrganisationStatus), organisation.OrganisationStatus.Value.ToString()),
            };

            A.CallTo(() => mapper.Map(organisation)).Returns(mappedOrganisationNameStatus);

            var result = await handler.HandleAsync(getAllOrganisationQuery);

            Assert.NotNull(result);
            Assert.Same(mappedOrganisationNameStatus, result);
        }

        public async Task HandleAsync_NotAvailableOrganisations_ShouldThrowArgumentException()
        {
            A.CallTo(() => context.Organisations).Returns(null);

            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(getAllOrganisationQuery));
        }

        private List<Organisation> CreateOrganisationList()
        {
            var organisation1 = A.Fake<Organisation>();
            var organisation2 = A.Fake<Organisation>();
            var organisation3 = A.Fake<Organisation>();

            return new List<Organisation>
            {
                organisation1,
                organisation2,
                organisation3
            };
        }
    }
 }