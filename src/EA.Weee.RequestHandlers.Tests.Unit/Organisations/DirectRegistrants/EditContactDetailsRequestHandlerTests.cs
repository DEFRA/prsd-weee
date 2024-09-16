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

    public class EditContactDetailsRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly EditContactDetailsRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private readonly Guid countryId = Guid.NewGuid();
        private readonly Guid userId = Guid.NewGuid();
        private readonly Country country;

        public EditContactDetailsRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();
            var dbContextHelper = new DbContextHelper();

            country = new Country(countryId, "UK");

            var countries = dbContextHelper.GetAsyncEnabledDbSet(new List<Country> { country });
            A.CallTo(() => weeeContext.Countries).Returns(countries);

            handler = new EditContactDetailsRequestHandler(
                authorization,
                genericDataAccess,
                weeeContext,
                systemDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant();

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
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
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

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_UpdatesCompanyDetails()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeTrue();
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_AddsNewAddress(bool existingAddress)
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant(existingAddress: existingAddress);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => genericDataAccess.Add(A<Address>.That.Matches(a => 
                a.Address1 == request.AddressData.Address1 && 
                a.Address2 == request.AddressData.Address2 &&
                a.TownOrCity == request.AddressData.TownOrCity &&
                a.CountyOrRegion == request.AddressData.CountyOrRegion &&
                a.Postcode == request.AddressData.Postcode &&
                a.Country.Equals(country)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_UpdatesAddress_WhenProvided(bool existingAddress)
        {
            // Arrange
            var request = CreateValidRequest(TestFixture.Create<string>());

            var directRegistrant = SetupValidDirectRegistrant(existingAddress: existingAddress);

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BusinessAddress.Address1.Should()
                .Be(request.AddressData.Address1);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BusinessAddress.Address2.Should()
                .Be(request.AddressData.Address2);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BusinessAddress.Country.Should()
                .Be(country);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BusinessAddress.CountyOrRegion.Should()
                .Be(request.AddressData.CountyOrRegion);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BusinessAddress.TownOrCity.Should()
                .Be(request.AddressData.TownOrCity);
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BusinessAddress.Postcode.Should()
                .Be(request.AddressData.Postcode);
        }

        [Fact]
        public async Task HandleAsync_DoesNotUpdateBrandName_WhenNotProvided()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant();

            // Act
            await handler.HandleAsync(request);

            // Assert
            directRegistrant.DirectProducerSubmissions.First().CurrentSubmission.BrandName.Should().BeNull();
        }

        private EditContactDetailsRequest CreateValidRequest(string brandNames = null)
        {
            var addressData = TestFixture.Build<AddressData>().With(a => a.CountryId, countryId).Create();

            var contactData = TestFixture.Build<ContactData>()
                .With(a => a.FirstName, "First")
                .With(a => a.LastName, "Last")
                .With(a => a.Position, "Pos")
                .Create();
            
            return new EditContactDetailsRequest(directRegistrantId, addressData, contactData);
        }

        private DirectRegistrant SetupValidDirectRegistrant(bool existingBrandName = false, bool existingAddress = false)
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

            var directRegistrant = new DirectRegistrant(A.Fake<Organisation>(),
                brandName, A.Fake<Contact>(), A.Fake<Address>(),
                A.Fake<AuthorisedRepresentative>(),
                A.CollectionOfFake<AdditionalCompanyDetails>(2).ToList());

            var directProducerSubmissionCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            directProducerSubmissionCurrentYear.CurrentSubmission =
                new DirectProducerSubmissionHistory(directProducerSubmissionCurrentYear, brandName, businessAddress);

            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionCurrentYear);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionNotCurrentYear);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }
    }
}