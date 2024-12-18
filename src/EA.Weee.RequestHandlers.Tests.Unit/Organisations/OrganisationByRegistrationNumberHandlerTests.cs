namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using DataAccess.DataAccess;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationByRegistrationNumberHandlerTests
    {
        private readonly IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
        private readonly IOrganisationDataAccess organisationDataAccess = A.Fake<IOrganisationDataAccess>();
        private readonly IMapper mapper = A.Fake<IMapper>();

        [Fact]
        public async Task OrganisationByRegistrationNumberHandler_Calls_GetByRegistrationNumber()
        {
            var message = new OrganisationByRegistrationNumberValue("456d");
            var handler = Handler();
            var result = await handler.HandleAsync(message);
            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationByRegistrationNumberHandler_Calls_EnsureCanAccessExternalArea()
        {
            var message = new OrganisationByRegistrationNumberValue("456d");
            var handler = Handler();
            var result = await handler.HandleAsync(message);
            A.CallTo(() => authorization.EnsureCanAccessExternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationByRegistrationNumberHandler_ReturnsEmptyListIfNotExists()
        {
            var message = new OrganisationByRegistrationNumberValue("456d");
            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber))
                .Returns(Task.FromResult(new List<Organisation>()));
            var handler = Handler();
            var result = await handler.HandleAsync(message);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationByRegistrationNumberHandler_MapsDataForMultipleOrganisations()
        {
            var message = new OrganisationByRegistrationNumberValue("456d456789");
            var org1 = Domain.Organisation.Organisation.CreateRegisteredCompany("Company1", message.RegistrationNumber, "s1");
            var org2 = Domain.Organisation.Organisation.CreateRegisteredCompany("Company2", message.RegistrationNumber, "s2");
            var expectedOrgs = new List<Organisation> { org1, org2 };

            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber))
                .Returns(Task.FromResult(expectedOrgs));

            var handler = Handler();
            var result = await handler.HandleAsync(message);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._))
                .MustHaveHappened(2, Times.Exactly);
        }

        private OrganisationByRegistrationNumberHandler Handler()
        {
            return new OrganisationByRegistrationNumberHandler(authorization, organisationDataAccess, mapper);
        }
    }
}