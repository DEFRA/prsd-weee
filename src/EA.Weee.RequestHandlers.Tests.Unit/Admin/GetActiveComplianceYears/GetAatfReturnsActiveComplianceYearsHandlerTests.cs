namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetActiveComplianceYears
{
    using System.Security;
    using System.Threading.Tasks;
    using FakeItEasy;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using RequestHandlers.Security;
    using Requests.Admin.GetActiveComplianceYears;
    using Xunit;

    public class GetAatfReturnsActiveComplianceYearsHandlerTests
    {
        private readonly IGetAatfReturnsActiveComplianceYearsDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetAatfReturnsActiveComplianceYearsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetAatfReturnsActiveComplianceYearsDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_UserIsNotAuthorized_ThrowsSecurityException_AndDoesNotUseDataAccess()
        {
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(() => Handler().HandleAsync(A.Dummy<GetAatfReturnsActiveComplianceYears>()));

            A.CallTo(() => dataAccess.Get()).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_UserIsAuthorized_UsesDataAccess()
        {
            await Handler().HandleAsync(A.Dummy<GetAatfReturnsActiveComplianceYears>());

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
        }

        private GetAatfReturnsActiveComplianceYearsHandler Handler()
        {
            return new GetAatfReturnsActiveComplianceYearsHandler(authorization, dataAccess);
        }
    }
}
