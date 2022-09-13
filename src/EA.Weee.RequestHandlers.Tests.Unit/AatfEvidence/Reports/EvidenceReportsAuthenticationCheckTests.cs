namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Reports
{
    using System;
    using AutoFixture;
    using FakeItEasy;
    using RequestHandlers.AatfEvidence.Reports;
    using RequestHandlers.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceReportsAuthenticationCheckTests : SimpleUnitTestBase
    {
        private readonly EvidenceReportsAuthenticationCheck authenticationChecker;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;

        public EvidenceReportsAuthenticationCheckTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();

            authenticationChecker = new EvidenceReportsAuthenticationCheck(authorization, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNullOrganisationIds_InternalAccessShouldBeChecked()
        {
            //arrange
            var request = new TestRequest(null, 
                null,
                null,
                TestFixture.Create<int>());

            //act
            await authenticationChecker.EnsureIsAuthorised(request);

            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRecipientOrganisationId_OrganisationAccessShouldBeChecked()
        {
            //arrange
            var recipientOrganisationId = TestFixture.Create<Guid>();

            var request = new TestRequest(recipientOrganisationId, 
                TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                TestFixture.Create<int>());

            //act
            await authenticationChecker.EnsureIsAuthorised(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(recipientOrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenOriginatingOrganisationId_OrganisationAccessShouldBeChecked()
        {
            //arrange
            var originatingOrganisationId = TestFixture.Create<Guid>();

            var request = new TestRequest(TestFixture.Create<Guid>(),
                originatingOrganisationId,
                TestFixture.Create<Guid>(),
                TestFixture.Create<int>());

            //act
            await authenticationChecker.EnsureIsAuthorised(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(originatingOrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfId_AatfShouldBeRetrieved()
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();

            var request = new TestRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                aatfId,
                TestFixture.Create<int>());

            //act
            await authenticationChecker.EnsureIsAuthorised(request);

            A.CallTo(() => genericDataAccess.GetById<Aatf>(aatfId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfId_AatfOrganisationAccessShouldBeChecked()
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();
            var organisationId = TestFixture.Create<Guid>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.OrganisationId).Returns(organisationId);
            A.CallTo(() => genericDataAccess.GetById<Aatf>(A<Guid>._)).Returns(aatf);

            var request = new TestRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(),
                aatfId,
                TestFixture.Create<int>());

            //act
            await authenticationChecker.EnsureIsAuthorised(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(organisationId)).MustHaveHappenedOnceExactly();
        }

        private class TestRequest : GetEvidenceReportBaseRequest
        {
            public TestRequest(Guid? recipientOrganisationId, Guid? originatorOrganisationId, Guid? aatfId, int complianceYear) : base(recipientOrganisationId, originatorOrganisationId, aatfId, complianceYear)
            {
            }
        }
    }
}
