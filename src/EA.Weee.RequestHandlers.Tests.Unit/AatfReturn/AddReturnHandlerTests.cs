namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNet.Identity;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Specification;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class AddReturnUploadHandlerTests
    {
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private AddReturnHandler handler;

        public AddReturnUploadHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userContext = A.Fake<IUserContext>();

            handler = new AddReturnHandler(weeeAuthorization, returnDataAccess, organisationDataAccess, genericDataAccess, userContext);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                A.Dummy<IUserContext>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                A.Dummy<IUserContext>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_DataAccessSubmitsIsCalled()
        {
            const int year = 2019;
            const int quarter = 1;

            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Quarter = quarter, Year = year, FacilityType = Core.AatfReturn.FacilityType.Aatf };

            var @return = A.Dummy<Return>();
            var organisation = new Organisation();
            var userId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.Submit(A<Return>.That.Matches(c => c.Quarter.Year == year && (int)c.Quarter.Q == quarter && c.Organisation.Equals(organisation) && c.CreatedById.Equals(userId.ToString())))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_OrganisationShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();
            var organisation = A.Fake<Organisation>();
            var request = new AddReturn { OrganisationId = organisationId, FacilityType = Core.AatfReturn.FacilityType.Aatf };

            A.CallTo(() => organisation.Id).Returns(organisationId);

            await handler.HandleAsync(request);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
