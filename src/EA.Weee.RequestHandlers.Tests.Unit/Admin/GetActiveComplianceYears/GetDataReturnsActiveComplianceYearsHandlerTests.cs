namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetActiveComplianceYears
{
    using System.Security;
    using System.Threading.Tasks;
    using FakeItEasy;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using RequestHandlers.Security;
    using Requests.Admin.GetActiveComplianceYears;
    using Xunit;

    public class GetDataReturnsActiveComplianceYearsHandlerTests
    {
        private readonly IGetDataReturnsActiveComplianceYearsDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetDataReturnsActiveComplianceYearsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetDataReturnsActiveComplianceYearsDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_UserIsNotAuthorized_ThrowsSecurityException_AndDoesNotUseDataAccess()
        {
            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            await
                Assert.ThrowsAsync<SecurityException>(
                    () => Handler().HandleAsync(A.Dummy<GetDataReturnsActiveComplianceYears>()));

            A.CallTo(() => dataAccess.Get())
                .MustNotHaveHappened();
        }

        private GetDataReturnsActiveComplianceYearsHandler Handler()
        {
            return new GetDataReturnsActiveComplianceYearsHandler(authorization, dataAccess);
        }
    }
}
