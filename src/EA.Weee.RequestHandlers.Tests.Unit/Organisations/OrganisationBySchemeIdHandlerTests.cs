namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Security;
    using DataAccess.DataAccess;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Xunit;

    public class OrganisationBySchemeIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMapper mapper;

        public OrganisationBySchemeIdHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async void IfAUserDoesNotHaveAccessToEitherSchemeOrInternalArea_ASecurityExceptionIsThrown_AndNoDataIsRetrieved()
        {
            var message = new OrganisationBySchemeId(Guid.NewGuid());

            A.CallTo(() => authorization.CheckInternalOrSchemeAccess(message.SchemeId))
                .Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(() => Handler().HandleAsync(message));

            A.CallTo(() => organisationDataAccess.GetBySchemeId(A<Guid>._))
                .MustNotHaveHappened();
        }

        private OrganisationBySchemeIdHandler Handler()
        {
            return new OrganisationBySchemeIdHandler(authorization, organisationDataAccess, mapper);
        }
    }
}
