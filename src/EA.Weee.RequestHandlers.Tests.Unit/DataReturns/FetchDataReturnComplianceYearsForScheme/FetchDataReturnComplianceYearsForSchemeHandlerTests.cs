namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.FetchDataReturnComplianceYearsForScheme
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.RequestHandlers.DataReturns.FetchDataReturnComplianceYearsForScheme;
    using FakeItEasy;
    using RequestHandlers.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchDataReturnComplianceYearsForSchemeHandlerTests
    {     
        private readonly IFetchDataReturnComplianceYearsForSchemeDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public FetchDataReturnComplianceYearsForSchemeHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IFetchDataReturnComplianceYearsForSchemeDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_NoAccessToScheme_ThrowsSecurityException()
        {
            Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");
           
            Requests.DataReturns.FetchDataReturnComplianceYearsForScheme request = new Requests.DataReturns.FetchDataReturnComplianceYearsForScheme(pcsId);
            A.CallTo(() => dataAccess.FetchSchemeByOrganisationIdAsync(pcsId)).Returns(A.Dummy<Domain.Scheme.Scheme>());

            A.CallTo(() => authorization.EnsureSchemeAccess(A<Guid>._))
               .Throws<SecurityException>();

            await
                Assert.ThrowsAsync<SecurityException>(
                    () => Handler().HandleAsync(request));

            A.CallTo(() => dataAccess.GetDataReturnComplianceYearsForScheme(pcsId))
                .MustNotHaveHappened();
        }

        private FetchDataReturnComplianceYearsForSchemeHandler Handler()
        {
            return new FetchDataReturnComplianceYearsForSchemeHandler(authorization, dataAccess);
        }
    } 
}
