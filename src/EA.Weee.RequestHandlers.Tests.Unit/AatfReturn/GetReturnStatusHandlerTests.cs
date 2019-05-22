namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;
    using ReturnStatus = Domain.AatfReturn.ReturnStatus;

    public class GetReturnStatusHandlerTests
    {
        private GetReturnStatusHandler handler;

        private readonly IMapper mapper;
        private readonly IUserContext userContext;
        private readonly IReturnDataAccess returnDataAccess;

        public GetReturnStatusHandlerTests()
        {
            userContext = A.Fake<IUserContext>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMapper>();

            handler = new GetReturnStatusHandler(new AuthorizationBuilder()
                .AllowExternalAreaAccess()
                .AllowOrganisationAccess().Build(),
                returnDataAccess,
                mapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetReturnStatusHandler(authorization,
                returnDataAccess,
                mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturnStatus>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(A.Fake<Return>());

            handler = new GetReturnStatusHandler(authorization,
                returnDataAccess,
                mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturnStatus>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenReturnId_ReturnShouldBeRetrieved()
        {
            var message = new GetReturnStatus(Guid.NewGuid());
            var @return = GetReturn();

            A.CallTo(() => returnDataAccess.GetById(message.ReturnId)).Returns(@return);

            await handler.HandleAsync(message);

            A.CallTo(() => returnDataAccess.GetById(message.ReturnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnCouldNotBeFound_ArgumentExceptionExpected()
        {
            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns((Return)null);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturnStatus>());

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_ReturnStatusShouldBeMapped()
        {
            var message = new GetReturnStatus(Guid.NewGuid());
            var @return = GetReturn();

            A.CallTo(() => returnDataAccess.GetById(message.ReturnId)).Returns(@return);

            await handler.HandleAsync(message);

            A.CallTo(() => mapper.Map<EA.Weee.Core.AatfReturn.ReturnStatus>(@return.ReturnStatus)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnStatus_MappedReturnStatusShouldBeReturned()
        {
            var message = new GetReturnStatus(Guid.NewGuid());
            var @return = GetReturn();

            foreach (var enumVar in Enum.GetValues(typeof(EA.Weee.Core.AatfReturn.ReturnStatus)))
            {
                A.CallTo(() => returnDataAccess.GetById(message.ReturnId)).Returns(@return);
                A.CallTo(() => mapper.Map<EA.Weee.Core.AatfReturn.ReturnStatus>(A<ReturnStatus>._)).Returns((EA.Weee.Core.AatfReturn.ReturnStatus)enumVar);

                var result = await handler.HandleAsync(message);

                result.ReturnStatus.Should().Be((EA.Weee.Core.AatfReturn.ReturnStatus)enumVar);
            }
        }

        [Fact]
        public async Task HandleAsync_GivenReturnOrganisation_OrganisationIdShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var message = new GetReturnStatus(Guid.NewGuid());
            var @return = A.Fake<Return>();
            var organisation = A.Fake<Organisation>();
            var @operator = new Operator(organisation);

            A.CallTo(() => @return.Operator).Returns(@operator);
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => returnDataAccess.GetById(message.ReturnId)).Returns(@return);

            var result = await handler.HandleAsync(message);

            result.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task HandleAsync_GivenNoOtherCreatedReturnsInYearAndQuarter_OtherInProgressReturnShouldBeFalse()
        {
            var message = new GetReturnStatus(Guid.NewGuid());
            var @return = A.Fake<Return>();

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => returnDataAccess.GetByComplianceYearAndQuarter(@return)).Returns(new List<Return>());

            var result = await handler.HandleAsync(message);

            result.OtherInProgressReturn.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenThereAreOtherCreatedReturnsInYearAndQuarter_OtherInProgressReturnShouldBeTrue()
        {
            var message = new GetReturnStatus(Guid.NewGuid());
            var @return = A.Fake<Return>();

            var returnsForYear = new List<Return>()
            {
                GetReturn()
            };

            A.CallTo(() => returnDataAccess.GetById(A<Guid>._)).Returns(@return);
            A.CallTo(() => returnDataAccess.GetByComplianceYearAndQuarter(@return)).Returns(returnsForYear);

            var result = await handler.HandleAsync(message);

            result.OtherInProgressReturn.Should().BeTrue();
        }

        public Return GetReturn()
        {
            return new Return(A.Fake<Operator>(), A.Fake<Quarter>(), "me") { ReturnStatus = ReturnStatus.Created };
        }
    }
}
