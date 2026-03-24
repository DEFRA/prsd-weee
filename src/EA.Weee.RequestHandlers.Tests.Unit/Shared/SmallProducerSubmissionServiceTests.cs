namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task GetSmallProducerSubmissionData_WhenCurrentYearSubmissionExists_ReturnsSmallProducerSubmissionData(bool hasAuthorisedRep, bool brandName)
        {
            var authorisedRepId = hasAuthorisedRep ? Guid.NewGuid() : (Guid?)null;
            var directRegistrant = SetupValidDirectRegistrant(true, authorisedRepId, brandName);

            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.HasAuthorisedRepresentitive.Should().Be(hasAuthorisedRep);
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().Be(submissionHistoryData);
            result.CurrentSystemYear.Should().Be(ComplianceYear);

            if (brandName)
            {
                result.EeeBrandNames.Should().Be(directRegistrant.BrandName.Name);
            }
            else
            {
                result.EeeBrandNames.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            var directRegistrant = SetupValidDirectRegistrant(false);
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

            result.CurrentSubmission.Should().BeNull();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_WhenCurrentYearSubmissionExists_CallsMapperCorrectly()
        {
            var directRegistrant = SetupValidDirectRegistrant(true);

            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

            // Assert
            result.AuthorisedRepresentitiveData.Should().BeNull();
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool hasCurrentYearSubmission = false, Guid? hasAuthorisedRep = null, bool hasBrandName = false)
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

            if (hasBrandName)
            {
                A.CallTo(() => directRegistrant.BrandName).Returns(new BrandName(TestFixture.Create<string>()));
                A.CallTo(() => directRegistrant.BrandNameId).Returns(Guid.NewGuid());
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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

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
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, true);

            // Assert
            result.ProducerRegistrationNumber.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_ExternalUser_PreviousYear_AddsSubmissionToHistory()
        {
            // Arrange
            var submissionYear = ComplianceYear - 1;
            var registeredProducer = A.Fake<RegisteredProducer>();
            var directRegistrant = SetupValidDirectRegistrant(hasCurrentYearSubmission: false);
            var directProducerSubmission = new DirectProducerSubmission(directRegistrant, registeredProducer, submissionYear);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmission);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, internalUser: false);

            // Assert
            result.SubmissionHistory.Should().ContainKey(submissionYear);
            result.SubmissionHistory[submissionYear].Should().Be(submissionHistoryData);
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_ExternalUser_CurrentYearCompleteSubmission_AddsSubmissionToHistory()
        {
            // Arrange
            var registeredProducer = A.Fake<RegisteredProducer>();
            var directRegistrant = SetupValidDirectRegistrant(hasCurrentYearSubmission: false);
            var directProducerSubmission = new DirectProducerSubmission(directRegistrant, registeredProducer, ComplianceYear);
            directProducerSubmission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete;
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmission);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, internalUser: false);

            // Assert
            result.SubmissionHistory.Should().ContainKey(ComplianceYear);
            result.SubmissionHistory[ComplianceYear].Should().Be(submissionHistoryData);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetSmallProducerSubmissionData_InternalAndExternalUser_CurrentYearReturnedSubmission_AddsSubmissionToHistory(bool yes)
        {
            // Arrange
            var registeredProducer = A.Fake<RegisteredProducer>();
            var directRegistrant = SetupValidDirectRegistrant(hasCurrentYearSubmission: yes);
            var directProducerSubmission = new DirectProducerSubmission(directRegistrant, registeredProducer, ComplianceYear);
            directProducerSubmission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Returned;
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmission);

            var submissionHistoryData = A.Fake<SmallProducerSubmissionHistoryData>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionHistoryData>(A<DirectProducerSubmissionSource>._)).Returns(submissionHistoryData);

            // Act
            var result = await service.GetSmallProducerSubmissionData(directRegistrant, internalUser: true);

            // Assert
            result.SubmissionHistory.Should().ContainKey(ComplianceYear);
            result.SubmissionHistory[ComplianceYear].Should().Be(submissionHistoryData);
        }

        [Theory]
        [InlineData("2026-03-31", 32.00)] // Before April 1st - old fee
        [InlineData("2026-04-01", 33.48)] // On April 1st - new fee
        [InlineData("2026-04-02", 33.48)] // After April 1st - new fee
        [InlineData("2026-12-31", 33.48)] // End of year - new fee
        public async Task GetSmallProducerSubmissionData_EnglandOrganisation_ReturnsCorrectFeeBasedOnDate(
    string dateString, decimal expectedAmount)
        {
            // Arrange
            var testDate = DateTime.Parse(dateString);
            var directRegistrant = SetupDirectRegistrantWithCountry("UK - England");

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, expectedAmount);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert
                result.DirectRegistrantChargeAmount.Should().Be(expectedAmount);
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026, true, testDate)).MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Theory]
        [InlineData("2026-03-31", 32.00)] // Before April 1st - old fee
        [InlineData("2026-04-01", 33.48)] // On April 1st - new fee
        [InlineData("2026-04-02", 33.48)] // After April 1st - new fee
        public async Task GetSmallProducerSubmissionData_NonUKOrganisation_ReturnsCorrectFeeBasedOnDate(
            string dateString, decimal expectedAmount)
        {
            // Arrange
            var testDate = DateTime.Parse(dateString);
            var directRegistrant = SetupDirectRegistrantWithCountry("France");

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, expectedAmount);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert
                result.DirectRegistrantChargeAmount.Should().Be(expectedAmount);
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026, true, testDate)).MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Theory]
        [InlineData("UK - Scotland", "2026-03-31")]
        [InlineData("UK - Scotland", "2026-04-01")]
        [InlineData("UK - Scotland", "2026-12-31")]
        [InlineData("UK - Wales", "2026-03-31")]
        [InlineData("UK - Wales", "2026-04-01")]
        [InlineData("UK - Wales", "2026-12-31")]
        [InlineData("UK - Northern Ireland", "2026-03-31")]
        [InlineData("UK - Northern Ireland", "2026-04-01")]
        [InlineData("UK - Northern Ireland", "2026-12-31")]
        public async Task GetSmallProducerSubmissionData_ScotlandWalesNI_FeeAlwaysRemains30(
            string countryName, string dateString)
        {
            // Arrange
            var testDate = DateTime.Parse(dateString);
            var directRegistrant = SetupDirectRegistrantWithCountry(countryName);

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, false, 30.00m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, false, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert
                result.DirectRegistrantChargeAmount.Should().Be(30.00m,
                    $"{countryName} should have £30 fee on {dateString}");
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026, false, testDate)).MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_EnglandBeforeApril2026_CallsDataAccessWithCorrectParameters()
        {
            // Arrange
            var testDate = new DateTime(2026, 3, 31);
            var directRegistrant = SetupDirectRegistrantWithCountry("UK - England");

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, 32.00m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert
                result.DirectRegistrantChargeAmount.Should().Be(32.00m);
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026, // complianceYear
                    true, // isNonUk (England is treated as non-UK for fee purposes)
                    testDate)) // asOfDate
                    .MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_EnglandOnApril2026_CallsDataAccessWithCorrectParameters()
        {
            // Arrange
            var testDate = new DateTime(2026, 4, 1);
            var directRegistrant = SetupDirectRegistrantWithCountry("UK - England");

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, 33.48m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert
                result.DirectRegistrantChargeAmount.Should().Be(33.48m);
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026, // complianceYear
                    true, // isNonUk (England is treated as non-UK for fee purposes)
                    testDate)) // asOfDate = April 1st
                    .MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Theory]
        [InlineData("Germany")]
        [InlineData("France")]
        [InlineData("Spain")]
        [InlineData("United States")]
        [InlineData("China")]
        public async Task GetSmallProducerSubmissionData_OverseasOrganisations_UseIsNonUkTrue(string countryName)
        {
            // Arrange
            var testDate = new DateTime(2026, 4, 1);
            var directRegistrant = SetupDirectRegistrantWithCountry(countryName);

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, 33.48m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert - verify IsNonUk = true is used for overseas countries
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026,
                    true, // isNonUk should be true for non-UK countries
                    testDate))
                    .MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_NullBusinessAddress_UsesDefaultHigherFee()
        {
            // Arrange
            var testDate = new DateTime(2026, 4, 1);
            var directRegistrant = A.Fake<DirectRegistrant>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => organisation.BusinessAddress).Returns(null); // No business address
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, 33.00m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert - should default to higher fee (IsNonUk = true)
                result.DirectRegistrantChargeAmount.Should().Be(33.00m);
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026,
                    true, // defaults to IsNonUk = true when address is null
                    testDate))
                    .MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_NullCountry_UsesDefaultHigherFee()
        {
            // Arrange
            var testDate = new DateTime(2026, 4, 1);
            var directRegistrant = A.Fake<DirectRegistrant>();
            var organisation = A.Fake<Organisation>();
            var businessAddress = A.Fake<Address>();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => organisation.BusinessAddress).Returns(businessAddress);
            A.CallTo(() => businessAddress.Country).Returns(null); // No country
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2026, true, 33.00m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert - should default to higher fee (IsNonUk = true)
                result.DirectRegistrantChargeAmount.Should().Be(33.00m);
                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                    2026,
                    true, // defaults to IsNonUk = true when country is null
                    testDate))
                    .MustHaveHappenedOnceExactly();
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task GetSmallProducerSubmissionData_2025_UsesCorrectFee()
        {
            // Arrange - Test that 2025 fees still work correctly
            var testDate = new DateTime(2025, 6, 1);
            var directRegistrant = SetupDirectRegistrantWithCountry("UK - England");

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var charge = CreateDirectRegistrantCharge(2025, true, 30.00m);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2025, true, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert - 2025 should use £30 for England
                result.DirectRegistrantChargeAmount.Should().Be(30.00m);
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        [Theory]
        [InlineData("uk - england")]
        [InlineData("UK - ENGLAND")]
        [InlineData("Uk - England")]
        [InlineData("UK - scotland")]
        [InlineData("UK - WALES")]
        public async Task GetSmallProducerSubmissionData_CountryNameCaseInsensitive_WorksCorrectly(string countryName)
        {
            // Arrange
            var testDate = new DateTime(2026, 4, 1);
            var directRegistrant = SetupDirectRegistrantWithCountry(countryName);

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(testDate);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                A<Guid>._, A<int>._)).Returns((DirectProducerSubmission)null);

            var isScotlandWalesNI = countryName.Equals("UK - Northern Ireland", StringComparison.OrdinalIgnoreCase) ||
                                    countryName.Equals("UK - Scotland", StringComparison.OrdinalIgnoreCase) ||
                                    countryName.Equals("UK - Wales", StringComparison.OrdinalIgnoreCase);
            
            var expectedIsNonUk = !isScotlandWalesNI;
            var expectedAmount = isScotlandWalesNI ? 30.00m : 33.48m;

            var charge = CreateDirectRegistrantCharge(2026, expectedIsNonUk, expectedAmount);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                2026, expectedIsNonUk, testDate))
                .Returns(charge);

            // Act
            EA.Prsd.Core.SystemTime.Freeze(testDate);
            try
            {
                var result = await service.GetSmallProducerSubmissionData(directRegistrant, false);

                // Assert - should work regardless of case
                result.DirectRegistrantChargeAmount.Should().Be(expectedAmount);
            }
            finally
            {
                EA.Prsd.Core.SystemTime.Unfreeze();
            }
        }

        /// <summary>
        /// Sets up a DirectRegistrant with a specific country for testing fee calculations
        /// </summary>
        private DirectRegistrant SetupDirectRegistrantWithCountry(string countryName)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            var organisation = A.Fake<Organisation>();
            var businessAddress = A.Fake<Address>();

            // Create a real Country object instead of faking it
            // Country constructor typically takes (Guid id, string name)
            var country = new Country(Guid.NewGuid(), countryName);

            A.CallTo(() => businessAddress.Country).Returns(country);
            A.CallTo(() => organisation.BusinessAddress).Returns(businessAddress);
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            return directRegistrant;
        }

        /// <summary>
        /// Creates a DirectRegistrantCharge for testing.
        /// Uses ObjectInstantiator to set private properties.
        /// </summary>
        private Domain.Lookup.DirectRegistrantCharge CreateDirectRegistrantCharge(
            int complianceYear,
            bool isNonUk,
            decimal amount,
            DateTime? effectiveFrom = null)
        {
            var charge = ObjectInstantiator<Domain.Lookup.DirectRegistrantCharge>.CreateNew();

            ObjectInstantiator<Domain.Lookup.DirectRegistrantCharge>.SetProperty(c => c.Id, Guid.NewGuid(), charge);
            ObjectInstantiator<Domain.Lookup.DirectRegistrantCharge>.SetProperty(c => c.ComplianceYear, complianceYear, charge);
            ObjectInstantiator<Domain.Lookup.DirectRegistrantCharge>.SetProperty(c => c.IsNonUk, isNonUk, charge);
            ObjectInstantiator<Domain.Lookup.DirectRegistrantCharge>.SetProperty(c => c.ChargeAmount, amount, charge);
            ObjectInstantiator<Domain.Lookup.DirectRegistrantCharge>.SetProperty(c => c.EffectiveFrom, effectiveFrom ?? new DateTime(complianceYear, 1, 1), charge);

            return charge;
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
