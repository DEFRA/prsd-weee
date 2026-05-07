namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
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

    public class AddSignatoryAndCompleteRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly AddSignatoryAndCompleteRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid countryId = Guid.NewGuid();
        private readonly Guid userId = Guid.NewGuid();
        private readonly Country country;
        private readonly Contact contact = new Contact("First", "Last", "Pos");
        private DirectProducerSubmission directProducerSubmissionCurrentYear;

        public AddSignatoryAndCompleteRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();
            var dbContextHelper = new DbContextHelper();

            country = new Country(countryId, "UK");

            var countries = dbContextHelper.GetAsyncEnabledDbSet(new List<Country> { country });
            A.CallTo(() => weeeContext.Countries).Returns(countries);

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(SystemTime.UtcNow);

            handler = new AddSignatoryAndCompleteRequestHandler(
                authorization,
                genericDataAccess,
                weeeContext,
                systemDataAccess,
                smallProducerDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant(true, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = CreateValidRequest();
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenDirectRegistrantNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = CreateValidRequest();
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId))
                .Returns(Task.FromResult<DirectRegistrant>(null));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentYearSubmission_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = new DirectRegistrant();
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            A.CallTo(() =>
                smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                    SystemTime.UtcNow.Year)).Returns<DirectProducerSubmission>(null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_AddSignatoryAndComplete()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeTrue();
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);

            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_SetsSubmissionDateAndStatus()
        {
            // Arrange
            var date = new DateTime(2024, 1, 1, 12, 10, 1);
            SystemTime.Freeze(date);

            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);
            var currentSubmission = directRegistrant.DirectProducerSubmissions.First().CurrentSubmission;
            var systemDate = new DateTime(2024, 1, 1);
            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(Task.FromResult(systemDate));

            // Act
            await handler.HandleAsync(request);

            // Assert
            currentSubmission.SubmittedDate.Value.Should().Be(date);
            directRegistrant.DirectProducerSubmissions.First().DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Complete);
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_DoesNotOverwriteRootBrandName_WhenSubmissionHasBrandName()
        {
            // Arrange — root DirectRegistrant has its own brand name;
            //           submission history has a separate brand name instance (set up independently in SetupValidDirectRegistrant)
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(existingBrandName: true, existingAddress: true);
            var originalRootBrandName = directRegistrant.BrandName?.Name;

            // Act
            await handler.HandleAsync(request);

            // Assert — root BrandName must remain unchanged; per-year data stays in submission history only
            directRegistrant.BrandName?.Name.Should().Be(originalRootBrandName,
                "completing a submission must not sync the submission history BrandName back to the root DirectRegistrant");
        }

        [Fact]
        public async Task HandleAsync_DoesNotOverwriteRootAuthorisedRepresentative_WhenSubmissionHasAuthRep()
        {
            // Arrange — root DirectRegistrant has "oldTradingName", submission history has a different auth rep
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(existingBrandName: true, existingAddress: true, useAuthRep: true);
            var originalTradingName = directRegistrant.AuthorisedRepresentative?.OverseasProducerTradingName;
            directProducerSubmissionCurrentYear.CurrentSubmission.AuthorisedRepresentative =
                new AuthorisedRepresentative(TestFixture.Create<string>(), TestFixture.Create<string>(), A.Fake<ProducerContact>());

            // Act
            await handler.HandleAsync(request);

            // Assert — root AuthorisedRepresentative must remain unchanged
            directRegistrant.AuthorisedRepresentative?.OverseasProducerTradingName.Should().Be(originalTradingName,
                "completing a submission must not sync the submission history AuthorisedRepresentative back to the root DirectRegistrant");
        }

        [Fact]
        public async Task HandleAsync_DoesNotOverwriteRootOrganisationDetails_WhenSubmissionHasCompanyDetails()
        {
            // Arrange — root Organisation has "companyName"/"tradingName", submission history has different values
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(existingBrandName: true, existingAddress: true);
            var originalName = directRegistrant.Organisation.Name;
            var originalTradingName = directRegistrant.Organisation.TradingName;
            directProducerSubmissionCurrentYear.CurrentSubmission.CompanyName = TestFixture.Create<string>();
            directProducerSubmissionCurrentYear.CurrentSubmission.TradingName = TestFixture.Create<string>();

            // Act
            await handler.HandleAsync(request);

            // Assert — root Organisation.Name and TradingName must not be overwritten from submission history
            directRegistrant.Organisation.Name.Should().Be(originalName,
                "completing a submission must not sync the submission history CompanyName back to the root Organisation");
            directRegistrant.Organisation.TradingName.Should().Be(originalTradingName,
                "completing a submission must not sync the submission history TradingName back to the root Organisation");
        }

        [Fact]
        public async Task HandleAsync_DoesNotOverwriteRootOrganisationBusinessAddress_WhenSubmissionHasAddress()
        {
            // Arrange — root Organisation has its own business address
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(existingBrandName: true, existingAddress: true);
            var originalAddress = directRegistrant.Organisation.BusinessAddress;

            // Act
            await handler.HandleAsync(request);

            // Assert — root Organisation.BusinessAddress must not be replaced with submission history address
            directRegistrant.Organisation.BusinessAddress.Should().BeSameAs(originalAddress,
                "completing a submission must not sync the submission history BusinessAddress back to the root Organisation");
        }

        [Fact]
        public async Task HandleAsync_DoesNotOverwriteRootContact_WhenSubmissionHasContact()
        {
            // Arrange — root DirectRegistrant has its own contact
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(existingBrandName: true, existingAddress: true);
            var originalContact = directRegistrant.Contact;

            // Act
            await handler.HandleAsync(request);

            // Assert — root DirectRegistrant.Contact must not be replaced with submission history contact
            directRegistrant.Contact.Should().BeSameAs(originalContact,
                "completing a submission must not sync the submission history Contact back to the root DirectRegistrant");
        }

        [Fact]
        public async Task HandleAsync_NoBrandNameOrAuthorisedRepresentative_CompletesSuccessfully()
        {
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(existingBrandName: false, existingAddress: true, useAuthRep: false);

            directProducerSubmissionCurrentYear.CurrentSubmission.AuthorisedRepresentative = null;
            directProducerSubmissionCurrentYear.CurrentSubmission.BrandName = null;

            var result = await handler.HandleAsync(request);

            result.Should().BeTrue();
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private AddSignatoryAndCompleteRequest CreateValidRequest(string brandNames = null)
        {
            var contactData = TestFixture.Build<ContactData>()
                .With(a => a.FirstName, contact.FirstName)
                .With(a => a.LastName, contact.LastName)
                .With(a => a.Position, contact.Position)
                .Create();

            return new AddSignatoryAndCompleteRequest(directRegistrantId, contactData);
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool existingBrandName = false, bool existingAddress = false, bool useAuthRep = true)
        {
            // Root DirectRegistrant brand name — independent instance
            BrandName rootBrandName = null;
            if (existingBrandName)
            {
                rootBrandName = new BrandName(TestFixture.Create<string>());
            }

            // Submission history brand name — separate instance with a different name
            // to prevent shared-entity mutation via OverwriteWhereNull corrupting the test
            BrandName historyBrandName = null;
            if (existingBrandName)
            {
                historyBrandName = new BrandName(TestFixture.Create<string>());
            }

            Address businessAddress = null;
            if (existingAddress)
            {
                businessAddress = ValueObjectInitializer.CreateAddress(TestFixture.Create<AddressData>(), country);
            }

            AuthorisedRepresentative authorisedRepresentative = null;
            if (useAuthRep)
            {
                authorisedRepresentative =
                    new AuthorisedRepresentative("oldName", "oldTradingName", A.Fake<ProducerContact>());
            }

            var directRegistrantAddress = new Address("address1", "address2", "town", "county", "gu21",
                new Country(Guid.NewGuid(), "country"), "1245", "email@email.com", "http://", "456789");

            var directRegistrant = new DirectRegistrant(
                Organisation.CreateDirectRegistrantCompany(Domain.Organisation.OrganisationType.Partnership, "companyName", "tradingName", "1231234"),
                rootBrandName,
                new Contact("First", "Last", "Position"),
                directRegistrantAddress,
                authorisedRepresentative,
                A.CollectionOfFake<AdditionalCompanyDetails>(2).ToList());

            directProducerSubmissionCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            // Use historyBrandName (separate instance) so that root and history never share the same entity.
            // Sharing the same instance would allow OverwriteWhereNull calls on the history to silently
            // mutate directRegistrant.BrandName, making root-overwrite tests unreliable.
            directProducerSubmissionCurrentYear.CurrentSubmission =
                new DirectProducerSubmissionHistory(directProducerSubmissionCurrentYear, historyBrandName, businessAddress)
                {
                    CompanyName = TestFixture.Create<string>()
                };

            directProducerSubmissionCurrentYear.CurrentSubmission.AddOrUpdateContact(contact);

            var submissionAddress = new Address("address2", "address2", "town", "county", "gu21",
                new Country(Guid.NewGuid(), "country"), "1245", "email@email.com", "http://", "456789");

            directProducerSubmissionCurrentYear.CurrentSubmission.AddOrUpdateContactAddress(submissionAddress);

            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionCurrentYear);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionNotCurrentYear);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            A.CallTo(() =>
                smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                    SystemTime.UtcNow.Year)).Returns(directProducerSubmissionCurrentYear);

            return directRegistrant;
        }
    }
}