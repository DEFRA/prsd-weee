namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class SmallProducerSubmissionHistoryDataMapTests
    {
        private readonly IMapper mapper;
        private readonly SmallProducerSubmissionHistoryDataMap map;
        private readonly Guid directRegistrantId = Guid.NewGuid();

        public SmallProducerSubmissionHistoryDataMapTests()
        {
            mapper = A.Fake<IMapper>();
            map = new SmallProducerSubmissionHistoryDataMap(mapper);
        }

        [Fact]
        public void Map_WithNullSource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => map.Map(null));
        }

        [Fact]
        public void Map_WhenCurrentYearSubmissionExists_MapsAllPropertiesCorrectly()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            SetupCurrentSubmissionFakes(currentYearSubmission.CurrentSubmission);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.Should().NotBeNull();
            VerifyAllPropertiesMapped(result, currentYearSubmission.CurrentSubmission);
        }

        [Fact]
        public void Map_WhenNoCurrentSubmissionData_FallsBackToDirectRegistrantData()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            SetupDirectRegistrantFallbackData(directRegistrant);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.Should().NotBeNull();
            VerifyFallbackDataUsed(result, directRegistrant);
        }

        [Fact]
        public void Map_WhenSellingTechniqueTypeIsSet_MapsCorrectly()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            currentYearSubmission.CurrentSubmission.SellingTechniqueType = (int)SellingTechniqueType.Both;

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.Should().NotBeNull();
            result.SellingTechnique.Should().Be(SellingTechniqueType.Both);
        }

        [Fact]
        public void Map_WithNullValues_HandlesGracefully()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            SetupNullValues(currentYearSubmission.CurrentSubmission);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.Should().NotBeNull();
            result.EEEBrandNames.Should().BeEmpty();
            result.CompanyName.Should().Be(directRegistrant.Organisation.Name);
            result.TradingName.Should().Be(directRegistrant.Organisation.TradingName);
            result.CompanyRegistrationNumber.Should().Be(directRegistrant.Organisation.CompanyRegistrationNumber);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_SetsHasPaidCorrectly(bool paymentFinished)
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            A.CallTo(() => currentYearSubmission.PaymentFinished).Returns(paymentFinished);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.HasPaid.Should().Be(paymentFinished);
        }

        [Fact]
        public void Map_SetsRegistrationDateCorrectly_WhenPaymentIsFinished()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            var expectedDate = new DateTime(2024, 1, 1);
            A.CallTo(() => currentYearSubmission.PaymentFinished).Returns(true);
            A.CallTo(() => currentYearSubmission.FinalPaymentSession).Returns(new PaymentSession { UpdatedAt = expectedDate });

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.RegistrationDate.Should().Be(expectedDate);
        }

        [Fact]
        public void Map_SetsRegistrationDateToNull_WhenPaymentIsNotFinished()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            A.CallTo(() => currentYearSubmission.PaymentFinished).Returns(false);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.RegistrationDate.Should().BeNull();
        }

        [Fact]
        public void Map_SetsPaymentReferenceCorrectly_WhenFinalPaymentSessionExists()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            const string expectedReference = "REF123";
            A.CallTo(() => currentYearSubmission.FinalPaymentSessionId).Returns(Guid.NewGuid());
            A.CallTo(() => currentYearSubmission.FinalPaymentSession).Returns(new PaymentSession { PaymentReference = expectedReference });

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.PaymentReference.Should().Be(expectedReference);
        }

        [Fact]
        public void Map_SetsPaymentReferenceToEmptyString_WhenNoFinalPaymentSession()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            A.CallTo(() => currentYearSubmission.FinalPaymentSessionId).Returns(null);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.PaymentReference.Should().BeEmpty();
        }

        [Fact]
        public void Map_SetsSubmittedDateCorrectly()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            var expectedDate = new DateTime(2024, 1, 1);
            A.CallTo(() => currentYearSubmission.CurrentSubmission.SubmittedDate).Returns(expectedDate);

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.SubmittedDate.Should().Be(expectedDate);
        }

        [Fact]
        public void Map_SetsProducerRegistrationNumberCorrectly()
        {
            var directRegistrant = CreateDirectRegistrant(true);
            var currentYearSubmission = CreateCurrentYearSubmission(directRegistrant);
            const string expectedRegistrationNumber = "WEE/AB1234CD";
            A.CallTo(() => currentYearSubmission.RegisteredProducer).Returns(new RegisteredProducer(expectedRegistrationNumber, 2024));

            var result = map.Map(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission));

            result.ProducerRegistrationNumber.Should().Be(expectedRegistrationNumber);
        }

        private DirectRegistrant CreateDirectRegistrant(bool hasCurrentYearSubmission)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.AuthorisedRepresentativeId).Returns(Guid.NewGuid());

            var submissions = new List<DirectProducerSubmission>();
            if (hasCurrentYearSubmission)
            {
                submissions.Add(CreateCurrentYearSubmission(directRegistrant));
            }
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            return directRegistrant;
        }

        private DirectProducerSubmission CreateCurrentYearSubmission(DirectRegistrant directRegistrant)
        {
            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => currentYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year);
            A.CallTo(() => currentYearSubmission.CurrentSubmission).Returns(A.Fake<DirectProducerSubmissionHistory>());
            A.CallTo(() => currentYearSubmission.FinalPaymentSession).Returns(A.Fake<PaymentSession>());
            return currentYearSubmission;
        }

        private void SetupCurrentSubmissionFakes(DirectProducerSubmissionHistory currentSubmissionHistory)
        {
            A.CallTo(() => currentSubmissionHistory.EeeOutputReturnVersion).Returns(A.Fake<EeeOutputReturnVersion>());
            A.CallTo(() => currentSubmissionHistory.BusinessAddress).Returns(A.Fake<Address>());
            A.CallTo(() => currentSubmissionHistory.BusinessAddressId).Returns(Guid.NewGuid());
            A.CallTo(() => currentSubmissionHistory.BrandName).Returns(new BrandName("Test Brand"));
            A.CallTo(() => currentSubmissionHistory.BrandNameId).Returns(Guid.NewGuid());
            A.CallTo(() => currentSubmissionHistory.CompanyName).Returns("Test Company");
            A.CallTo(() => currentSubmissionHistory.TradingName).Returns("Test Trading Name");
            A.CallTo(() => currentSubmissionHistory.CompanyRegistrationNumber).Returns("12345678");
            A.CallTo(() => currentSubmissionHistory.SellingTechniqueType).Returns(Domain.Producer.Classfication.SellingTechniqueType.Both.Value);
            A.CallTo(() => currentSubmissionHistory.ContactAddressId).Returns(Guid.NewGuid());
            A.CallTo(() => currentSubmissionHistory.ContactId).Returns(Guid.NewGuid());
            A.CallTo(() => currentSubmissionHistory.Contact).Returns(A.Fake<Contact>());
            A.CallTo(() => currentSubmissionHistory.AuthorisedRepresentative).Returns(A.Fake<AuthorisedRepresentative>());
            A.CallTo(() => currentSubmissionHistory.AuthorisedRepresentativeId).Returns(Guid.NewGuid());
            A.CallTo(() => currentSubmissionHistory.ServiceOfNoticeAddressId).Returns(Guid.NewGuid());
        }

        private void SetupDirectRegistrantFallbackData(DirectRegistrant directRegistrant)
        {
            A.CallTo(() => directRegistrant.Organisation.BusinessAddress).Returns(A.Fake<Address>());
            A.CallTo(() => directRegistrant.BrandName).Returns(new BrandName("Fallback Brand"));
            A.CallTo(() => directRegistrant.BrandNameId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(Organisation.CreateDirectRegistrantCompany(Domain.Organisation.OrganisationType.RegisteredCompany, "Fallback Company", "Fallback Trading Name", "87654321"));
            A.CallTo(() => directRegistrant.Contact).Returns(A.Fake<Contact>());
            A.CallTo(() => directRegistrant.Address).Returns(A.Fake<Address>());
            A.CallTo(() => directRegistrant.AuthorisedRepresentative).Returns(A.Fake<AuthorisedRepresentative>());
            A.CallTo(() => directRegistrant.AuthorisedRepresentativeId).Returns(Guid.NewGuid());
        }

        private void SetupNullValues(DirectProducerSubmissionHistory currentSubmissionHistory)
        {
            A.CallTo(() => currentSubmissionHistory.BrandName).Returns(null);
            A.CallTo(() => currentSubmissionHistory.CompanyName).Returns(null);
            A.CallTo(() => currentSubmissionHistory.TradingName).Returns(null);
            A.CallTo(() => currentSubmissionHistory.CompanyRegistrationNumber).Returns(null);
        }

        private void VerifyAllPropertiesMapped(SmallProducerSubmissionHistoryData result, DirectProducerSubmissionHistory currentSubmissionHistory)
        {
            result.EEEDetailsComplete.Should().BeTrue();
            result.OrganisationDetailsComplete.Should().BeTrue();
            result.ServiceOfNoticeComplete.Should().BeTrue();
            result.ContactDetailsComplete.Should().BeTrue();
            result.EEEBrandNames.Should().Be("Test Brand");
            result.CompanyName.Should().Be("Test Company");
            result.TradingName.Should().Be("Test Trading Name");
            result.CompanyRegistrationNumber.Should().Be("12345678");
            result.SellingTechnique.Should().Be((SellingTechniqueType)Domain.Producer.Classfication.SellingTechniqueType.Both.Value);

            VerifyMapperCalls(currentSubmissionHistory);
        }

        private void VerifyFallbackDataUsed(SmallProducerSubmissionHistoryData result, DirectRegistrant directRegistrant)
        {
            result.EEEBrandNames.Should().Be("Fallback Brand");
            result.CompanyName.Should().Be("Fallback Company");
            result.TradingName.Should().Be("Fallback Trading Name");
            result.CompanyRegistrationNumber.Should().Be("87654321");

            VerifyMapperCalls(directRegistrant);
        }

        private void VerifyMapperCalls(DirectProducerSubmissionHistory currentSubmissionHistory)
        {
            A.CallTo(() => mapper.Map<Address, AddressData>(currentSubmissionHistory.BusinessAddress)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Contact, ContactData>(currentSubmissionHistory.Contact)).MustHaveHappened();
            A.CallTo(() => mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(currentSubmissionHistory.AuthorisedRepresentative)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Address, AddressData>(currentSubmissionHistory.ServiceOfNoticeAddress)).MustHaveHappened();
            A.CallTo(() => mapper.Map<EeeOutputReturnVersion, IList<Eee>>(currentSubmissionHistory.EeeOutputReturnVersion)).MustHaveHappened();
        }

        private void VerifyMapperCalls(DirectRegistrant directRegistrant)
        {
            A.CallTo(() => mapper.Map<Address, AddressData>(directRegistrant.Organisation.BusinessAddress)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Contact, ContactData>(directRegistrant.Contact)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Address, AddressData>(directRegistrant.Address)).MustHaveHappened();
            A.CallTo(() => mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(directRegistrant.AuthorisedRepresentative)).MustHaveHappened();
        }
    }
}