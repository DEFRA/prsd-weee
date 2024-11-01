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
        public async Task HandleAsync_UpdatesBrandName_WhenBrandNameExists()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);
            var name = TestFixture.Create<string>();
            directProducerSubmissionCurrentYear.CurrentSubmission.AddOrUpdateBrandName(new BrandName(name));

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.BrandName.Name.Should().Be(name);
        }

        [Fact]
        public async Task HandleAsync_UpdatesAuthorisedRepresentative_WhenExists()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);

            var name = TestFixture.Create<string>();
            var tradingName = TestFixture.Create<string>();
            directProducerSubmissionCurrentYear.CurrentSubmission.AuthorisedRepresentative =
                new AuthorisedRepresentative(name, tradingName, A.Fake<ProducerContact>());

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.AuthorisedRepresentative.OverseasProducerTradingName.Should().Be(tradingName);
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
        public async Task HandleAsync_UpdatesOrganisationDetails()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);
            directProducerSubmissionCurrentYear.CurrentSubmission.CompanyName = TestFixture.Create<string>();
            directProducerSubmissionCurrentYear.CurrentSubmission.TradingName = TestFixture.Create<string>();

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.Organisation.Name.Should().Be(directProducerSubmissionCurrentYear.CurrentSubmission.CompanyName);
            directRegistrant.Organisation.TradingName.Should().Be(directProducerSubmissionCurrentYear.CurrentSubmission.TradingName);
        }

        [Fact]
        public async Task HandleAsync_UpdatesAddress_WhenExists()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.Organisation.BusinessAddress.Should().BeEquivalentTo(directProducerSubmissionCurrentYear.CurrentSubmission.BusinessAddress);
        }

        [Fact]
        public async Task HandleAsync_UpdatesContact_WhenExists()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(true, true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.Contact.Should()
                .BeEquivalentTo(directProducerSubmissionCurrentYear.CurrentSubmission.Contact);
        }

        [Fact]
        public async Task HandleAsync_NoBrandNameOrAuthorisedRepresentative()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(false, true, useAuthRep: false); // No brand name or authorised representative
            
            directProducerSubmissionCurrentYear.CurrentSubmission.AuthorisedRepresentative = null;
            directProducerSubmissionCurrentYear.CurrentSubmission.BrandName = null;

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeTrue();
            directRegistrant.BrandName.Should().BeNull();
            directRegistrant.AuthorisedRepresentative.Should().BeNull();
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private AddSignatoryAndCompleteRequest CreateValidRequest(string brandNames = null)
        {
            var contactData = TestFixture.Build<ContactData>()
                .With(a => a.FirstName, contact.FirstName)
                .With(a => a.LastName, contact.LastName)
                .With(a => a.Position, contact.Position)
                .Create();
            
            return new AddSignatoryAndCompleteRequest(directRegistrantId,  contactData);
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool existingBrandName = false, bool existingAddress = false, bool useAuthRep = true)
        {
            BrandName brandName = null;
            if (existingBrandName)
            {
                brandName = new BrandName(TestFixture.Create<string>());
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
                new Country(Guid.NewGuid(), "country"), "1245", "email@email.com", "http://");

            var directRegistrant = new DirectRegistrant(Organisation.CreateDirectRegistrantCompany(Domain.Organisation.OrganisationType.Partnership, "companyName", "tradingName", "1231234"),
                brandName, new Contact("First", "Last", "Position"), directRegistrantAddress,
                authorisedRepresentative,
                A.CollectionOfFake<AdditionalCompanyDetails>(2).ToList());

            directProducerSubmissionCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            directProducerSubmissionCurrentYear.CurrentSubmission =
                new DirectProducerSubmissionHistory(directProducerSubmissionCurrentYear, brandName, businessAddress)
                    {
                        CompanyName = TestFixture.Create<string>()
                    };

            directProducerSubmissionCurrentYear.CurrentSubmission.AddOrUpdateContact(contact);

            var submissionAddress = new Address("address2", "address2", "town", "county", "gu21",
                new Country(Guid.NewGuid(), "country"), "1245", "email@email.com", "http://");

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