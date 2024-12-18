namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetActiveComplianceYears
{
    using EA.Weee.DataAccess.DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin.GetActiveComplianceYears;
    using RequestHandlers.Security;
    using Requests.Admin.GetActiveComplianceYears;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetDataReturnsActiveComplianceYearsHandlerTests
    {
        private readonly IGetDataReturnsActiveComplianceYearsDataAccess dataAccess = A.Fake<IGetDataReturnsActiveComplianceYearsDataAccess>();
        private readonly IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
        private readonly IGetDirectProducerSubmissionActiveComplianceYearsDataAccess directProducerDataAccess = A.Fake<IGetDirectProducerSubmissionActiveComplianceYearsDataAccess>();

        [Fact]
        public async Task HandleAsync_UserIsNotAuthorized_ThrowsSecurityException_AndDoesNotUseDataAccess()
        {
            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(
                () => Handler().HandleAsync(new GetDataReturnsActiveComplianceYears(false)));

            A.CallTo(() => dataAccess.Get()).MustNotHaveHappened();
            A.CallTo(() => directProducerDataAccess.Get(1)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_UserIsAuthorized_UsesDataAccess()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021 });

            await Handler().HandleAsync(new GetDataReturnsActiveComplianceYears(false));

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => directProducerDataAccess.Get(1)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_IncludeDirectRegistrantSubmissions_UsesAllYears()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021 });
            A.CallTo(() => directProducerDataAccess.Get(1)).Returns(new List<int> { 2019, 2021 });

            var result = await Handler().HandleAsync(new GetDataReturnsActiveComplianceYears(true));

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => directProducerDataAccess.Get(1)).MustHaveHappenedOnceExactly();

            Assert.Equal(new List<int> { 2021, 2020, 2019 }, result);
        }

        [Fact]
        public async Task HandleAsync_ReturnsDistinctSortedYears()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021, 2019 });
            A.CallTo(() => directProducerDataAccess.Get(1)).Returns(new List<int> { 2019, 2021, 2022 });

            var result = await Handler().HandleAsync(new GetDataReturnsActiveComplianceYears(true));

            Assert.Equal(new List<int> { 2022, 2021, 2020, 2019 }, result);
        }

        [Fact]
        public async Task HandleAsync_IncludeDirectRegistrantSubmissionsFalse_OnlyUsesDataAccess()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021 });
            A.CallTo(() => directProducerDataAccess.Get(1)).Returns(new List<int> { 2019, 2022 });

            var result = await Handler().HandleAsync(new GetDataReturnsActiveComplianceYears(false));

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => directProducerDataAccess.Get(1)).MustNotHaveHappened();

            Assert.Equal(new List<int> { 2021, 2020 }, result);
        }

        private GetDataReturnsActiveComplianceYearsHandler Handler()
        {
            return new GetDataReturnsActiveComplianceYearsHandler(authorization, dataAccess, directProducerDataAccess);
        }
    }
}
