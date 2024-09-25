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
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationByRegistrationNumberHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMapper mapper;

        public OrganisationByRegistrationNumberHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            mapper = A.Fake<IMapper>();
        }

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
        public async Task OrganisationByRegistrationNumberHandler_ReturnsNullIfNotExists()
        {
            var message = new OrganisationByRegistrationNumberValue("456d");

            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber)).Returns(Task.FromResult<Domain.Organisation.Organisation>(null));

            var handler = Handler();

            var result = await handler.HandleAsync(message);

            result.Should().BeNull();

            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationByRegistrationNumberHandler_MapsData()
        {
            var message = new OrganisationByRegistrationNumberValue("456d456789");

            var expected = Domain.Organisation.Organisation.CreateRegisteredCompany("s", message.RegistrationNumber, "s");

            A.CallTo(() => organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber)).Returns(expected);

            var handler = Handler();

            var result = await handler.HandleAsync(message);

            result.Should().NotBeNull();

            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(expected))
                .MustHaveHappenedOnceExactly();
        }

        private OrganisationByRegistrationNumberHandler Handler()
        {
            return new OrganisationByRegistrationNumberHandler(authorization, organisationDataAccess, mapper);
        }
    }
}
