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
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class AddReturnUploadHandlerTests
    {
        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddReturnRequestHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturnRequest>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            var handler = new AddReturnRequestHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturnRequest>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_DataAccessSubmitsIsCalled()
        {
            const int year = 2019;
            const int quarter = 1;

            var request = new AddReturnRequest { OrganisationId = Guid.NewGuid(), Quarter = quarter,  Year = year };

            var returnDataAccess = A.Fake<IReturnDataAccess>();
            var organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            var @return = A.Dummy<Return>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => organisationDataAccess.GetById(request.OrganisationId)).Returns(organisation);

            var handler = new AddReturnRequestHandler(A.Dummy<IWeeeAuthorization>(), returnDataAccess, organisationDataAccess);

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.Submit(A<Return>.That.Matches(c => c.Quarter.Year == year && (int)c.Quarter.Q == quarter))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
