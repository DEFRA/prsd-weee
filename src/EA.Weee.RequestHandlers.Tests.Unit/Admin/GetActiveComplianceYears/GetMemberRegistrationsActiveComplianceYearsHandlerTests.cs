namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetActiveComplianceYears
{
    using System.Security;
    using System.Threading.Tasks;
    using FakeItEasy;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using RequestHandlers.Security;
    using Requests.Admin.GetActiveComplianceYears;
    using Xunit;

    public class GetMemberRegistrationsActiveComplianceYearsHandlerTests
    {
        private readonly IGetMemberRegistrationsActiveComplianceYearsDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetMemberRegistrationsActiveComplianceYearsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetMemberRegistrationsActiveComplianceYearsDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_UserIsNotAuthorized_ThrowsSecurityException_AndDoesNotUseDataAccess()
        {
            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            await
                Assert.ThrowsAsync<SecurityException>(
                    () => Handler().HandleAsync(A.Dummy<GetMemberRegistrationsActiveComplianceYears>()));

            A.CallTo(() => dataAccess.Get())
                .MustNotHaveHappened();
        }

        private GetMemberRegistrationsActiveComplianceYearsHandler Handler()
        {
            return new GetMemberRegistrationsActiveComplianceYearsHandler(authorization, dataAccess);
        }
    }
}
