namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
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
    using EA.Weee.Domain.DataReturns;
    using Xunit;

    public class GetSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly GetSmallProducerSubmissionHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();

        public GetSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);
            handler = new GetSmallProducerSubmissionHandler(authorization, genericDataAccess, mapper, systemDataAccess);
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

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.HasAuthorisedRepresentitive.Should().Be(hasAuthorisedRep);
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().NotBeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(false);

            var result = await handler.HandleAsync(request);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_SetsOrganisationDetailsCompleteCorrectly(bool hasBusinessAddressId, bool expectedOrganisationDetailsComplete)
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            var currentSubmissionHistory = A.Fake<DirectProducerSubmissionHistory>();
            A.CallTo(() => currentSubmissionHistory.BusinessAddressId).Returns(hasBusinessAddressId ? (Guid?)Guid.NewGuid() : null);
            A.CallTo(() => currentYearSubmission.CurrentSubmission).Returns(currentSubmissionHistory);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.First()).Returns(currentYearSubmission);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.OrganisationDetailsComplete.Should().Be(expectedOrganisationDetailsComplete);
        }

        [Fact]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_MapsAllPropertiesCorrectly()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            var currentSubmissionHistory = A.Fake<DirectProducerSubmissionHistory>();
            A.CallTo(() => currentYearSubmission.CurrentSubmission).Returns(currentSubmissionHistory);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.First()).Returns(currentYearSubmission);

            SetupCurrentSubmissionFakes(currentSubmissionHistory);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            VerifyAllPropertiesMapped(result.CurrentSubmission, currentSubmissionHistory);
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentSubmissionData_FallsBackToDirectRegistrantData()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            var currentSubmissionHistory = A.Fake<DirectProducerSubmissionHistory>();
            A.CallTo(() => currentYearSubmission.CurrentSubmission).Returns(currentSubmissionHistory);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.First()).Returns(currentYearSubmission);

            SetupDirectRegistrantFallbackData(directRegistrant);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            VerifyFallbackDataUsed(result.CurrentSubmission, directRegistrant);
        }

        [Fact]
        public async Task HandleAsync_WhenSellingTechniqueTypeIsSet_MapsCorrectly()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            var currentSubmissionHistory = A.Fake<DirectProducerSubmissionHistory>();
            A.CallTo(() => currentYearSubmission.CurrentSubmission).Returns(currentSubmissionHistory);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.First()).Returns(currentYearSubmission);
            A.CallTo(() => currentSubmissionHistory.SellingTechniqueType).Returns(Domain.Producer.Classfication.SellingTechniqueType.Both.Value);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.SellingTechnique.Should().Be(SellingTechniqueType.Both);
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

        private void SetupCurrentSubmissionFakes(DirectProducerSubmissionHistory currentSubmissionHistory)
        {
            var brandName = A.Fake<BrandName>();
            A.CallTo(() => brandName.Name).Returns("Test Brand");
            A.CallTo(() => currentSubmissionHistory.EeeOutputReturnVersion).Returns(A.Fake<EeeOutputReturnVersion>());
            A.CallTo(() => currentSubmissionHistory.BusinessAddress).Returns(A.Fake<Address>());
            A.CallTo(() => currentSubmissionHistory.BrandName).Returns(brandName);
            A.CallTo(() => currentSubmissionHistory.CompanyName).Returns("Test Company");
            A.CallTo(() => currentSubmissionHistory.TradingName).Returns("Test Trading Name");
            A.CallTo(() => currentSubmissionHistory.CompanyRegistrationNumber).Returns("12345678");
            A.CallTo(() => currentSubmissionHistory.SellingTechniqueType).Returns(Domain.Producer.Classfication.SellingTechniqueType.Both.Value);
            A.CallTo(() => currentSubmissionHistory.ContactAddress).Returns(A.Fake<Address>());
            A.CallTo(() => currentSubmissionHistory.Contact).Returns(A.Fake<Contact>());
            A.CallTo(() => currentSubmissionHistory.AuthorisedRepresentative).Returns(A.Fake<AuthorisedRepresentative>());
            A.CallTo(() => currentSubmissionHistory.ServiceOfNoticeAddress).Returns(A.Fake<Address>());
        }

        private void SetupDirectRegistrantFallbackData(DirectRegistrant directRegistrant)
        {
            var brandName = A.Fake<BrandName>();
            A.CallTo(() => brandName.Name).Returns("Fallback Brand");
            A.CallTo(() => directRegistrant.Organisation.BusinessAddress).Returns(A.Fake<Address>());
            A.CallTo(() => directRegistrant.BrandName).Returns(brandName);
            A.CallTo(() => directRegistrant.Organisation.Name).Returns("Fallback Company");
            A.CallTo(() => directRegistrant.Organisation.TradingName).Returns("Fallback Trading Name");
            A.CallTo(() => directRegistrant.Organisation.CompanyRegistrationNumber).Returns("87654321");
            A.CallTo(() => directRegistrant.Contact).Returns(A.Fake<Contact>());
            A.CallTo(() => directRegistrant.Address).Returns(A.Fake<Address>());
            A.CallTo(() => directRegistrant.AuthorisedRepresentative).Returns(A.Fake<AuthorisedRepresentative>());
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

            A.CallTo(() => mapper.Map<Domain.Organisation.Address, AddressData>(currentSubmissionHistory.BusinessAddress)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Contact, ContactData>(currentSubmissionHistory.Contact)).MustHaveHappened();
            A.CallTo(() => mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(currentSubmissionHistory.AuthorisedRepresentative)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Domain.Organisation.Address, AddressData>(currentSubmissionHistory.ServiceOfNoticeAddress)).MustHaveHappened();
            A.CallTo(() => mapper.Map<EeeOutputReturnVersion, IList<Eee>>(currentSubmissionHistory.EeeOutputReturnVersion)).MustHaveHappened();
        }

        private void VerifyFallbackDataUsed(SmallProducerSubmissionHistoryData result, DirectRegistrant directRegistrant)
        {
            result.EEEBrandNames.Should().Be("Fallback Brand");
            result.CompanyName.Should().Be("Fallback Company");
            result.TradingName.Should().Be("Fallback Trading Name");
            result.CompanyRegistrationNumber.Should().Be("87654321");

            A.CallTo(() => mapper.Map<Domain.Organisation.Address, AddressData>(directRegistrant.Organisation.BusinessAddress)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Contact, ContactData>(directRegistrant.Contact)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Domain.Organisation.Address, AddressData>(directRegistrant.Address)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Contact, ContactData>(directRegistrant.Contact)).MustHaveHappened();
            A.CallTo(() => mapper.Map<Domain.Organisation.Address, AddressData>(directRegistrant.Address)).MustHaveHappened();
            A.CallTo(() => mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(directRegistrant.AuthorisedRepresentative)).MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WithNullValues_HandlesGracefully()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            var currentSubmissionHistory = A.Fake<DirectProducerSubmissionHistory>();
            A.CallTo(() => currentYearSubmission.CurrentSubmission).Returns(currentSubmissionHistory);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.First()).Returns(currentYearSubmission);

            // Set up null values
            A.CallTo(() => currentSubmissionHistory.BrandName).Returns(null);
            A.CallTo(() => currentSubmissionHistory.CompanyName).Returns(null);
            A.CallTo(() => currentSubmissionHistory.TradingName).Returns(null);
            A.CallTo(() => currentSubmissionHistory.CompanyRegistrationNumber).Returns(null);

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.EEEBrandNames.Should().BeEmpty();
            result.CurrentSubmission.CompanyName.Should().Be(directRegistrant.Organisation.Name);
            result.CurrentSubmission.TradingName.Should().Be(directRegistrant.Organisation.TradingName);
            result.CurrentSubmission.CompanyRegistrationNumber.Should().Be(directRegistrant.Organisation.CompanyRegistrationNumber);
        }

        [Fact]
        public async Task HandleAsync_WithDifferentComplianceYear_ReturnsNull()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => currentYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year - 1); // Previous year
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.First()).Returns(currentYearSubmission);

            var result = await handler.HandleAsync(request);

            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WithMultipleSubmissions_ReturnsCurrentYearSubmission()
        {
            var request = new GetSmallProducerSubmission(directRegistrantId);
            var directRegistrant = SetupValidDirectRegistrant(true);
            var currentYearSubmission = A.Fake<DirectProducerSubmission>();
            var previousYearSubmission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => currentYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year);
            A.CallTo(() => previousYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year - 1);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission> { previousYearSubmission, currentYearSubmission });

            var result = await handler.HandleAsync(request);

            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
        }
    }
}