namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class SmallProducerSubmissionServiceTests : SimpleUnitTestBase
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly SmallProducerSubmissionService service;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private const int ComplianceYear = 2024;
        private DirectProducerSubmission currentYearSubmission;

        public SmallProducerSubmissionServiceTests()
        {
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(new DateTime(ComplianceYear, 1, 1));
            service = new SmallProducerSubmissionService(mapper, systemDataDataAccess, smallProducerDataAccess);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetSmallProducerSubmissionData_WhenCurrentYearSubmissionExists_ReturnsSmallProducerSubmissionData(bool hasAuthorisedRep)
        {
            var authorisedRepId = hasAuthorisedRep ? Guid.NewGuid() : (Guid?)null;
            var directRegistrant = SetupValidDirectRegistrant(true, authorisedRepId);

            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.HasAuthorisedRepresentitive.Should().Be(hasAuthorisedRep);
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().Be(submissionHistoryData);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            var directRegistrant = SetupValidDirectRegistrant(false);
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            result.CurrentSubmission.Should().BeNull();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_WhenCurrentYearSubmissionExists_CallsMapperCorrectly()
        {
            var directRegistrant = SetupValidDirectRegistrant(true);
            
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);
            
            await service.GetSmallProducerSubmissionData(directRegistrant);

            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>.That.Matches(s => 
                s.DirectRegistrant == directRegistrant &&
                s.DirectProducerSubmission.Equals(currentYearSubmission)))).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_IncludesContactData_WhenAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();
            var contact = A.Fake<Contact>();
            var contactData = A.Fake<ContactData>();

            A.CallTo(() => directRegistrant.Contact).Returns(contact);
            A.CallTo(() => mapper.Map<Contact, ContactData>(contact)).Returns(contactData);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.ContactData.Should().Be(contactData);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_ReturnsNullContactData_WhenNotAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.Contact).Returns(null);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.ContactData.Should().BeNull();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_IncludesAddressData_WhenAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();
            var address = A.Fake<Address>();
            var addressData = A.Fake<AddressData>();

            A.CallTo(() => directRegistrant.Address).Returns(address);
            A.CallTo(() => mapper.Map<Address, AddressData>(address)).Returns(addressData);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.ContactAddressData.Should().Be(addressData);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_ReturnsNullAddressData_WhenNotAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.Address).Returns(null);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.ContactAddressData.Should().BeNull();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_IncludesSubmissionHistory()
        {
            // Arrange
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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.SubmissionHistory.Should().HaveCount(3);
            result.SubmissionHistory.Should().ContainKeys(2021, 2022, 2023);
            result.SubmissionHistory.Should().ContainValue(submissionHistory1);
            result.SubmissionHistory.Should().ContainValue(submissionHistory2);
            result.SubmissionHistory.Should().ContainValue(submissionHistory3);
        }

        [Fact]
        public async Task HandleAsyncGetSmallProducerSubmissionData_ReturnsEmptySubmissionHistory_WhenNoSubmissions()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.SubmissionHistory.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_SetsCurrentSubmission_WhenAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant(true);
            CreateSubmission(ComplianceYear);

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(new DateTime(ComplianceYear, 1, 1));
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._))
                .Returns(new SmallProducerSubmissionHistoryData { ComplianceYear = ComplianceYear });

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.ComplianceYear.Should().Be(ComplianceYear);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_SetsCurrentSubmissionToNull_WhenNotAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(new DateTime(ComplianceYear, 1, 1));
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());
            A.CallTo(() =>
                smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                    ComplianceYear)).Returns<DirectProducerSubmission>(null);
            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.CurrentSubmission.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_IncludesAuthorisedRepresentitiveData_WhenAvailable()
        {
            // Arrange
            var authorisedRepId = Guid.NewGuid();
            var directRegistrant = SetupValidDirectRegistrant(true, authorisedRepId);
            var authorisedRepresentative = A.Fake<AuthorisedRepresentative>();
            var authorisedRepresentitiveData = A.Fake<AuthorisedRepresentitiveData>();

            A.CallTo(() => directRegistrant.AuthorisedRepresentative).Returns(authorisedRepresentative);
            A.CallTo(() => mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(authorisedRepresentative)).Returns(authorisedRepresentitiveData);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.AuthorisedRepresentitiveData.Should().Be(authorisedRepresentitiveData);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_ReturnsNullAuthorisedRepresentitiveData_WhenNotAvailable()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();

            A.CallTo(() => directRegistrant.AuthorisedRepresentative).Returns(null);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.AuthorisedRepresentitiveData.Should().BeNull();
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
                currentYearSubmission = A.Fake<DirectProducerSubmission>();
                A.CallTo(() =>
                    smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                        ComplianceYear)).Returns(currentYearSubmission);

                submissions.Add(currentYearSubmission);
            }
            else
            {
                A.CallTo(() =>
                    smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                        ComplianceYear)).Returns<DirectProducerSubmission>(null);
            }

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_WhenSubmissionHistoryExists_SetsProducerRegistrationNumber()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();
            var registeredProducer = A.Fake<RegisteredProducer>();
            const string expectedRegNumber = "WEE/AB1234CD";

            var submission = CreateSubmission(2023);
            A.CallTo(() => submission.RegisteredProducer).Returns(registeredProducer);
            A.CallTo(() => registeredProducer.ProducerRegistrationNumber).Returns(expectedRegNumber);

            var submissions = new List<DirectProducerSubmission> { submission };
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.ProducerRegistrationNumber.Should().Be(expectedRegNumber);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_WhenNoSubmissionHistory_ReturnsEmptyProducerRegistrationNumber()
        {
            // Arrange
            var directRegistrant = SetupValidDirectRegistrant();
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant);

            // Assert
            result.ProducerRegistrationNumber.Should().BeEmpty();
        }

        private static DirectProducerSubmission CreateSubmission(int year)
        {
            var submission = A.Fake<DirectProducerSubmission>();
            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => submission.ComplianceYear).Returns(year);
            A.CallTo(() => submission.RegisteredProducer).Returns(registeredProducer);
            return submission;
        }
    }
}
