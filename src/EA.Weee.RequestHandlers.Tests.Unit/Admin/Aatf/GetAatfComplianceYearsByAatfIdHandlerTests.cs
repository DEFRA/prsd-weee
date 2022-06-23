namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.RequestHandlers.Admin.Aatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Aatf;
    using Xunit;

    public class GetAatfComplianceYearsByAatfIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess dataAccess;
        private GetAatfComplianceYearsByAatfIdHandler handler;

        public GetAatfComplianceYearsByAatfIdHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IAatfDataAccess>();

            handler = new GetAatfComplianceYearsByAatfIdHandler(authorization, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetAatfComplianceYearsByAatfIdHandler(authorization, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfComplianceYearsByAatfId>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetComplianceYearsForAatfByAatfIdShouldBeCalled()
        {
            var result = await handler.HandleAsync(A.Dummy<GetAatfComplianceYearsByAatfId>());

            A.CallTo(() => dataAccess.GetComplianceYearsForAatfByAatfId(A.Dummy<Guid>())).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenAatfId_AatfComplianceYearsMustBeReturned()
        {
            var complianceYears = A.CollectionOfDummy<short>(2).ToList();

            A.CallTo(() => dataAccess.GetComplianceYearsForAatfByAatfId(A.Dummy<Guid>())).Returns(complianceYears);

            var result = await handler.HandleAsync(A.Dummy<GetAatfComplianceYearsByAatfId>());

            result.Should().BeEquivalentTo(complianceYears);
        }
    }
}
