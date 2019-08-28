namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetActiveComplianceYears
{
    using FakeItEasy;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using RequestHandlers.Security;
    using Requests.Admin.GetActiveComplianceYears;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAatfActiveComplianceYearsHandlerTests
    {
        private readonly IGetAatfReturnsActiveComplianceYearsDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetAatfActiveComplianceYearsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetAatfReturnsActiveComplianceYearsDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_UserIsNotAuthorized_ThrowsSecurityException_AndDoesNotUseDataAccess()
        {
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(() => Handler().HandleAsync(A.Dummy<GetAatfAeActiveComplianceYears>()));

            A.CallTo(() => dataAccess.Get()).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_UserIsAuthorized_UsesDataAccess()
        {
            await Handler().HandleAsync(A.Dummy<GetAatfAeActiveComplianceYears>());

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.GetAatfAe()).MustHaveHappenedOnceExactly();
        }

        private GetAatfAeActiveComplianceYearsHandler Handler()
        {
            return new GetAatfAeActiveComplianceYearsHandler(authorization, dataAccess);
        }
    }
}
