namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNet.Identity;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Specification;
    using RequestHandlers.Factories;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;
    using FacilityType = Core.AatfReturn.FacilityType;
    using Organisation = Domain.Organisation.Organisation;

    public class AddReturnUploadHandlerTests
    {
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly IReturnFactoryDataAccess returnFactoryDataAccess;

        private AddReturnHandler handler;

        public AddReturnUploadHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userContext = A.Fake<IUserContext>();
            returnFactoryDataAccess = A.Fake<IReturnFactoryDataAccess>();

            handler = new AddReturnHandler(weeeAuthorization, returnDataAccess, genericDataAccess, userContext, returnFactoryDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                A.Dummy<IUserContext>(), 
                A.Dummy<IReturnFactoryDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                A.Dummy<IUserContext>(),
                A.Dummy<IReturnFactoryDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Theory]
        [InlineData(QuarterType.Q1)]
        [InlineData(QuarterType.Q2)]
        [InlineData(QuarterType.Q3)]
        [InlineData(QuarterType.Q4)]
        public async Task HandleAsync_GivenAddReturnRequest_DataAccessSubmitsIsCalled(QuarterType quarterType)
        {
            const int year = 2019;
            const int quarter = 1;

            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Quarter = quarterType, Year = year, FacilityType = Core.AatfReturn.FacilityType.Aatf };

            var @return = A.Dummy<Return>();
            var organisation = new Organisation();
            var userId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.Submit(A<Return>.That.Matches(c => c.Quarter.Year == request.Year && (int)c.Quarter.Q == (int)quarterType && c.Organisation.Equals(organisation) && c.CreatedById.Equals(userId.ToString())))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_OrganisationShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();
            var organisation = A.Fake<Organisation>();
            var request = new AddReturn { OrganisationId = organisationId, FacilityType = Core.AatfReturn.FacilityType.Aatf, Year = 2019, QuarterType = quarterType };

            A.CallTo(() => organisation.Id).Returns(organisationId);

            await handler.HandleAsync(request);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequestAndReturnAlreadyExists_InvalidOperationExceptionExpected()
        {
            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Year = 2019, Quarter = QuarterType.Q1, FacilityType = FacilityType.Aatf };

            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(request.OrganisationId, request.Year, (Domain.DataReturns.QuarterType)request.Quarter, request.FacilityType))
                .Returns(true);

            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            result.Should().BeOfType<InvalidOperationException>();

            A.CallTo(() => returnDataAccess.Submit(A<Return>._)).MustNotHaveHappened();
        }
    }
}
