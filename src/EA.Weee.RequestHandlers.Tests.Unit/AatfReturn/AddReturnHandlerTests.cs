namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.DataReturns;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Factories;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
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
        private readonly IReturnFactory returnFactory;
        private AddReturnHandler handler;
        private const int year = 2019;
        private const QuarterType quarter = QuarterType.Q1;

        public AddReturnUploadHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userContext = A.Fake<IUserContext>();
            returnFactoryDataAccess = A.Fake<IReturnFactoryDataAccess>();
            returnFactory = A.Fake<IReturnFactory>();

            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(A<Guid>._, A<int>._, A<EA.Weee.Domain.DataReturns.QuarterType>._, A<FacilityType>._)).Returns(false);
            A.CallTo(() => returnFactory.GetReturnQuarter(A<Guid>._, A<FacilityType>._)).Returns(new Quarter(year, quarter));

            handler = new AddReturnHandler(weeeAuthorization, returnDataAccess, genericDataAccess, userContext, returnFactoryDataAccess, returnFactory);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IGenericDataAccess>(),
                A.Dummy<IUserContext>(),
                A.Dummy<IReturnFactoryDataAccess>(),
                A.Dummy<IReturnFactory>());

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
                A.Dummy<IReturnFactoryDataAccess>(),
                A.Dummy<IReturnFactory>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Theory]
        [InlineData(QuarterType.Q1, FacilityType.Aatf)]
        [InlineData(QuarterType.Q2, FacilityType.Aatf)]
        [InlineData(QuarterType.Q3, FacilityType.Aatf)]
        [InlineData(QuarterType.Q4, FacilityType.Aatf)]
        [InlineData(QuarterType.Q1, FacilityType.Ae)]
        [InlineData(QuarterType.Q2, FacilityType.Ae)]
        [InlineData(QuarterType.Q3, FacilityType.Ae)]
        [InlineData(QuarterType.Q4, FacilityType.Ae)]
        public async Task HandleAsync_GivenAddReturnRequest_DataAccessSubmitsIsCalled(QuarterType quarterType, FacilityType facility)
        {
            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Quarter = quarterType, Year = year, FacilityType = Core.AatfReturn.FacilityType.Aatf };

            var @return = A.Dummy<Return>();
            var organisation = new Organisation();
            var userId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => returnFactory.GetReturnQuarter(A<Guid>._, A<FacilityType>._)).Returns(new Quarter(year, quarterType));

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.Submit(A<Return>.That.Matches(c => c.Quarter.Year == request.Year && (int)c.Quarter.Q == (int)quarterType && c.Organisation.Equals(organisation) && c.CreatedById.Equals(userId.ToString())))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_OrganisationShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();
            var organisation = A.Fake<Organisation>();
            var request = new AddReturn { OrganisationId = organisationId, FacilityType = Core.AatfReturn.FacilityType.Aatf, Year = year, Quarter = quarter };

            A.CallTo(() => organisation.Id).Returns(organisationId);

            await handler.HandleAsync(request);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequestAndReturnAlreadyExists_InvalidOperationExceptionExpected()
        {
            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Year = year, Quarter = quarter, FacilityType = FacilityType.Aatf };

            A.CallTo(() => returnFactoryDataAccess.HasReturnQuarter(request.OrganisationId, request.Year, (Domain.DataReturns.QuarterType)request.Quarter, request.FacilityType))
                .Returns(true);

            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            result.Should().BeOfType<InvalidOperationException>();
            A.CallTo(() => returnDataAccess.Submit(A<Return>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequestAndReturnYearThatShouldBeCreatedDoesNotMatch_InvalidOperationExceptionExpected()
        {
            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Year = year, Quarter = quarter, FacilityType = FacilityType.Aatf };

            A.CallTo(() => returnFactory.GetReturnQuarter(request.OrganisationId, request.FacilityType))
                .Returns(new Quarter(2020, QuarterType.Q1));

            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));
            result.Should().BeOfType<InvalidOperationException>();
            A.CallTo(() => returnDataAccess.Submit(A<Return>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequestAndQuarterThatShouldBeCreatedDoesNotMatch_InvalidOperationExceptionExpected()
        {
            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Year = year, Quarter = quarter, FacilityType = FacilityType.Aatf };

            A.CallTo(() => returnFactory.GetReturnQuarter(request.OrganisationId, request.FacilityType))
                .Returns(new Quarter(year, QuarterType.Q2));

            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));
            result.Should().BeOfType<InvalidOperationException>();
            A.CallTo(() => returnDataAccess.Submit(A<Return>._)).MustNotHaveHappened();
        }
    }
}
