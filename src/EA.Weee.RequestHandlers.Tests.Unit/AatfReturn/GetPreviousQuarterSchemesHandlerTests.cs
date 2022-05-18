namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class GetPreviousQuarterSchemesHandlerTests
    {
        private readonly IGenericDataAccess dataAccess;
        private GetPreviousQuarterSchemesHandler handler;
        private readonly Fixture fixture;

        public GetPreviousQuarterSchemesHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();

            dataAccess = A.Fake<IGenericDataAccess>();
            handler = new GetPreviousQuarterSchemesHandler(weeeAuthorization, dataAccess);

            fixture = new Fixture();
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetPreviousQuarterSchemesHandler(authorization, A.Dummy<IGenericDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetPreviousQuarterSchemes>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_ProvideOrganisationIdWithPreviousReturns_ReturnsPreviousSchemesForThatOrganisation()
        {
            Organisation requestedOrganisation = fixture.Build<Organisation>().With(o => o.OrganisationStatus, OrganisationStatus.Complete).Create();
            Organisation otherOrganisation = fixture.Build<Organisation>().With(o => o.OrganisationStatus, OrganisationStatus.Complete).Create();

            Quarter q1 = new Quarter(2019, QuarterType.Q1);
            Quarter q2 = new Quarter(2019, QuarterType.Q2);
            Quarter q3 = new Quarter(2019, QuarterType.Q3);
            Quarter q4 = new Quarter(2019, QuarterType.Q4);

            Return previousReturn = A.Fake<Return>();
            A.CallTo(() => previousReturn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => previousReturn.Quarter).Returns(q3);
            A.CallTo(() => previousReturn.Organisation).Returns(requestedOrganisation);
            A.CallTo(() => previousReturn.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            Return currentReturn = A.Fake<Return>();
            A.CallTo(() => currentReturn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => currentReturn.Quarter).Returns(q4);
            A.CallTo(() => currentReturn.Organisation).Returns(requestedOrganisation);
            A.CallTo(() => currentReturn.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            List<Return> returns = new List<Return>()
            {
                A.Fake<Return>(),
                A.Fake<Return>(),
                A.Fake<Return>(),
                A.Fake<Return>()
            };

            A.CallTo(() => returns.ElementAt(0).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(0).Quarter).Returns(q2);
            A.CallTo(() => returns.ElementAt(0).Organisation).Returns(requestedOrganisation);
            A.CallTo(() => returns.ElementAt(0).ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            A.CallTo(() => returns.ElementAt(1).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(1).Quarter).Returns(q1);
            A.CallTo(() => returns.ElementAt(1).Organisation).Returns(requestedOrganisation);
            A.CallTo(() => returns.ElementAt(1).ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            A.CallTo(() => returns.ElementAt(2).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(2).Quarter).Returns(q1);
            A.CallTo(() => returns.ElementAt(2).Organisation).Returns(otherOrganisation);
            A.CallTo(() => returns.ElementAt(2).ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            A.CallTo(() => returns.ElementAt(3).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(3).Quarter).Returns(q2);
            A.CallTo(() => returns.ElementAt(3).Organisation).Returns(otherOrganisation);
            A.CallTo(() => returns.ElementAt(3).ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            returns.Add(previousReturn);
            returns.Add(currentReturn);

            A.CallTo(() => dataAccess.GetAll<Return>()).Returns(returns);

            List<ReturnScheme> returnSchemes = new List<ReturnScheme>()
            {
                A.Fake<ReturnScheme>(),
                A.Fake<ReturnScheme>(),
                A.Fake<ReturnScheme>()
            };

            A.CallTo(() => returnSchemes.ElementAt(0).SchemeId).Returns(Guid.NewGuid());
            A.CallTo(() => returnSchemes.ElementAt(0).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returnSchemes.ElementAt(0).ReturnId).Returns(previousReturn.Id);

            A.CallTo(() => returnSchemes.ElementAt(1).SchemeId).Returns(Guid.NewGuid());
            A.CallTo(() => returnSchemes.ElementAt(1).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returnSchemes.ElementAt(1).ReturnId).Returns(previousReturn.Id);

            A.CallTo(() => returnSchemes.ElementAt(2).SchemeId).Returns(Guid.NewGuid());
            A.CallTo(() => returnSchemes.ElementAt(2).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returnSchemes.ElementAt(2).ReturnId).Returns(returns.FirstOrDefault(p => p.Organisation.Id == otherOrganisation.Id).Id);

            A.CallTo(() => dataAccess.GetAll<ReturnScheme>()).Returns(returnSchemes);

            GetPreviousQuarterSchemes request = new GetPreviousQuarterSchemes(requestedOrganisation.Id, currentReturn.Id);

            PreviousQuarterReturnResult result = await handler.HandleAsync(request);

            Assert.Equal(2, result.PreviousSchemes.Count);

            result.PreviousQuarter.Should().Be(new Core.DataReturns.Quarter(q3.Year, (Core.DataReturns.QuarterType)q3.Q));

            result.PreviousSchemes.Should().Contain(returnSchemes.Where(p => p.ReturnId == previousReturn.Id).Select(p => p.SchemeId));

            A.CallTo(() => dataAccess.GetAll<Return>()).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.GetAll<ReturnScheme>()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_ProvideOrganisationIdWithOutPreviousReturns_ReturnsEmptyList()
        {
            Organisation requestedOrganisation = fixture.Build<Organisation>().With(o => o.OrganisationStatus, OrganisationStatus.Complete).Create();
            Organisation otherOrganisation = fixture.Build<Organisation>().With(o => o.OrganisationStatus, OrganisationStatus.Complete).Create();

            Quarter q1 = new Quarter(2019, QuarterType.Q1);
            Quarter q2 = new Quarter(2019, QuarterType.Q2);
            Quarter q3 = new Quarter(2019, QuarterType.Q3);

            List<Return> returns = new List<Return>()
            {
                A.Fake<Return>(),
                A.Fake<Return>()
            };

            Return currentReturn = A.Fake<Return>();
            A.CallTo(() => currentReturn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => currentReturn.Quarter).Returns(q3);
            A.CallTo(() => currentReturn.Organisation).Returns(requestedOrganisation);
            A.CallTo(() => currentReturn.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            returns.Add(currentReturn);

            A.CallTo(() => returns.ElementAt(0).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(0).Quarter).Returns(q1);
            A.CallTo(() => returns.ElementAt(0).Organisation).Returns(otherOrganisation);
            A.CallTo(() => returns.ElementAt(0).ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            A.CallTo(() => returns.ElementAt(1).Id).Returns(Guid.NewGuid());
            A.CallTo(() => returns.ElementAt(1).Quarter).Returns(q2);
            A.CallTo(() => returns.ElementAt(1).Organisation).Returns(otherOrganisation);
            A.CallTo(() => returns.ElementAt(1).ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            A.CallTo(() => dataAccess.GetAll<Return>()).Returns(returns);

            List<ReturnScheme> returnSchemes = new List<ReturnScheme>();

            A.CallTo(() => dataAccess.GetAll<ReturnScheme>()).Returns(returnSchemes);

            GetPreviousQuarterSchemes request = new GetPreviousQuarterSchemes(requestedOrganisation.Id, currentReturn.Id);

            PreviousQuarterReturnResult result = await handler.HandleAsync(request);

            Assert.Equal(0, result.PreviousSchemes.Count);

            A.CallTo(() => dataAccess.GetAll<Return>()).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.GetAll<ReturnScheme>()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_PreviousReturnHasResubmissions_ReturnsEmptyList()
        {
            Organisation requestedOrganisation = fixture.Build<Organisation>().With(o => o.OrganisationStatus, OrganisationStatus.Complete).Create();
           
            Quarter q3 = new Quarter(2019, QuarterType.Q3);
            Quarter q4 = new Quarter(2019, QuarterType.Q4);

            Return previousReturn = A.Fake<Return>();
            A.CallTo(() => previousReturn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => previousReturn.Quarter).Returns(q3);
            A.CallTo(() => previousReturn.Organisation).Returns(requestedOrganisation);
            A.CallTo(() => previousReturn.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            Return previousResubmittedReturn = A.Fake<Return>();
            A.CallTo(() => previousResubmittedReturn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => previousResubmittedReturn.Quarter).Returns(q3);
            A.CallTo(() => previousResubmittedReturn.Organisation).Returns(requestedOrganisation);
            A.CallTo(() => previousResubmittedReturn.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            Return currentReturn = A.Fake<Return>();
            A.CallTo(() => currentReturn.Id).Returns(Guid.NewGuid());
            A.CallTo(() => currentReturn.Quarter).Returns(q4);
            A.CallTo(() => currentReturn.Organisation).Returns(requestedOrganisation);
            A.CallTo(() => currentReturn.ReturnStatus).Returns(Domain.AatfReturn.ReturnStatus.Submitted);

            List<Return> returns = new List<Return>();
      
            returns.Add(previousReturn);
            returns.Add(previousResubmittedReturn);
            returns.Add(currentReturn);

            A.CallTo(() => dataAccess.GetAll<Return>()).Returns(returns);

            List<ReturnScheme> returnSchemes = new List<ReturnScheme>();

            A.CallTo(() => dataAccess.GetAll<ReturnScheme>()).Returns(returnSchemes);

            GetPreviousQuarterSchemes request = new GetPreviousQuarterSchemes(requestedOrganisation.Id, currentReturn.Id);

            PreviousQuarterReturnResult result = await handler.HandleAsync(request);

            Assert.Equal(0, result.PreviousSchemes.Count);

            A.CallTo(() => dataAccess.GetAll<Return>()).MustHaveHappened(1, Times.Exactly);
        }
    }
}
