namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly GetSmallProducerSubmissionHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();

        public GetSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);
            handler = new GetSmallProducerSubmissionHandler(authorization, genericDataAccess, mapper, systemDataDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            SetupValidDirectRegistrant();

            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();

            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_ReturnsSmallProducerSubmissionData(bool hasAuthorisedRep)
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var authorisedRepId = hasAuthorisedRep ? Guid.NewGuid() : (Guid?)null;
            var directRegistrant = SetupValidDirectRegistrant(true, authorisedRepId);

            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.HasAuthorisedRepresentitive.Should().Be(hasAuthorisedRep);
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().Be(submissionHistoryData);
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(false);

            var result = await handler.HandleAsync(request);

            result.CurrentSubmission.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_CallsMapperCorrectly()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.FirstOrDefault(s => s.ComplianceYear == SystemTime.UtcNow.Year);

            await handler.HandleAsync(request);

            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>.That.Matches(s => s.DirectRegistrant == directRegistrant && 
                s.DirectProducerSubmission == currentYearSubmission))).MustHaveHappenedTwiceExactly();
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool hasCurrentYearSubmission = false, Guid? hasAuthorisedRep = null)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.AuthorisedRepresentativeId).Returns(hasAuthorisedRep);

            var submissions = new List<DirectProducerSubmission>();
            if (hasCurrentYearSubmission)
            {
                var currentYearSubmission = A.Fake<DirectProducerSubmission>();
                A.CallTo(() => currentYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year);
                submissions.Add(currentYearSubmission);
            }
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }

        [Fact]
        public async Task HandleAsync_IncludesContactData_WhenAvailable()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();
            var contact = A.Fake<Contact>();
            var contactData = A.Fake<ContactData>();

            A.CallTo(() => directRegistrant.Contact).Returns(contact);
            A.CallTo(() => mapper.Map<Contact, ContactData>(contact)).Returns(contactData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.ContactData.Should().Be(contactData);
        }

        [Fact]
        public async Task HandleAsync_ReturnsNullContactData_WhenNotAvailable()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.Contact).Returns(null);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.ContactData.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_IncludesAddressData_WhenAvailable()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();
            var address = A.Fake<Address>();
            var addressData = A.Fake<AddressData>();

            A.CallTo(() => directRegistrant.Address).Returns(address);
            A.CallTo(() => mapper.Map<Address, AddressData>(address)).Returns(addressData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.ContactAddressData.Should().Be(addressData);
        }

        [Fact]
        public async Task HandleAsync_ReturnsNullAddressData_WhenNotAvailable()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.Address).Returns(null);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.ContactAddressData.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_IncludesSubmissionHistory()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();
            var submissions = new List<DirectProducerSubmission>
            {
                CreateSubmission(2021),
                CreateSubmission(2022),
                CreateSubmission(2023)
            };

            var submissionHistory1 = TestFixture.Create<SmallProducerSubmissionHistoryData>();
            var submissionHistory2 = TestFixture.Create<SmallProducerSubmissionHistoryData>();
            var submissionHistory3 = TestFixture.Create<SmallProducerSubmissionHistoryData>();

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._))
                .ReturnsNextFromSequence(submissionHistory1, submissionHistory2, submissionHistory3);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.SubmissionHistory.Should().HaveCount(3);
            result.SubmissionHistory.Should().ContainKeys(2021, 2022, 2023);
            result.SubmissionHistory.Should().ContainValue(submissionHistory1);
            result.SubmissionHistory.Should().ContainValue(submissionHistory2);
            result.SubmissionHistory.Should().ContainValue(submissionHistory3);
        }

        [Fact]
        public async Task HandleAsync_ReturnsEmptySubmissionHistory_WhenNoSubmissions()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.SubmissionHistory.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleAsync_SetsCurrentSubmission_WhenAvailable()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();
            var currentYear = SystemTime.UtcNow.Year;
            var currentSubmission = CreateSubmission(currentYear);

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission> { currentSubmission });
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._))
                .Returns(new SmallProducerSubmissionHistoryData { ComplianceYear = currentYear });

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.ComplianceYear.Should().Be(currentYear);
        }

        [Fact]
        public async Task HandleAsync_SetsCurrentSubmissionToNull_WhenNotAvailable()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.CurrentSubmission.Should().BeNull();
        }

        private DirectProducerSubmission CreateSubmission(int year)
        {
            var submission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => submission.ComplianceYear).Returns(year);
            return submission;
        }
    }
}