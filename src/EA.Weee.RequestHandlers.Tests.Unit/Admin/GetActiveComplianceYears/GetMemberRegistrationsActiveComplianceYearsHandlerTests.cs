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

    public class GetMemberRegistrationsActiveComplianceYearsHandlerTests
    {
        private readonly IGetMemberRegistrationsActiveComplianceYearsDataAccess dataAccess = A.Fake<IGetMemberRegistrationsActiveComplianceYearsDataAccess>();
        private readonly IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
        private readonly IGetDirectProducerSubmissionActiveComplianceYearsDataAccess directProducerDataAccess = A.Fake<IGetDirectProducerSubmissionActiveComplianceYearsDataAccess>();

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
            A.CallTo(() => directProducerDataAccess.Get(0)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_UserIsAuthorized_UsesDataAccess()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021 });

            await Handler().HandleAsync(new GetMemberRegistrationsActiveComplianceYears(false));

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => directProducerDataAccess.Get(0)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_IncludeDirectRegistrantSubmissions_UsesAllYears()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021 });
            A.CallTo(() => directProducerDataAccess.Get(0)).Returns(new List<int> { 2019, 2021 });

            var result = await Handler().HandleAsync(new GetMemberRegistrationsActiveComplianceYears(true));

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => directProducerDataAccess.Get(0)).MustHaveHappenedOnceExactly();

            Assert.Equal(new List<int> { 2021, 2020, 2019 }, result);
        }

        [Fact]
        public async Task HandleAsync_ReturnsDistinctSortedYears()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021, 2019 });
            A.CallTo(() => directProducerDataAccess.Get(0)).Returns(new List<int> { 2019, 2021, 2022 });

            var result = await Handler().HandleAsync(new GetMemberRegistrationsActiveComplianceYears(true));

            Assert.Equal(new List<int> { 2022, 2021, 2020, 2019 }, result);
        }

        [Fact]
        public async Task HandleAsync_IncludeDirectRegistrantSubmissionsFalse_OnlyUsesDataAccess()
        {
            A.CallTo(() => dataAccess.Get()).Returns(new List<int> { 2020, 2021 });
            A.CallTo(() => directProducerDataAccess.Get(0)).Returns(new List<int> { 2019, 2022 });

            var result = await Handler().HandleAsync(new GetMemberRegistrationsActiveComplianceYears(false));

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => directProducerDataAccess.Get(0)).MustNotHaveHappened();

            Assert.Equal(new List<int> { 2021, 2020 }, result);
        }

        private GetMemberRegistrationsActiveComplianceYearsHandler Handler()
        {
            return new GetMemberRegistrationsActiveComplianceYearsHandler(authorization, dataAccess, directProducerDataAccess);
        }
    }
}
