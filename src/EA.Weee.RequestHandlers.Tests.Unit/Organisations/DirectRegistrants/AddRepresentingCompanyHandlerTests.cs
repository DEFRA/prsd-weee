namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class AddRepresentingCompanyHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly AddRepresentingCompanyHandler handler;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly Guid countryId = Guid.NewGuid();
        private readonly Country country;

        public AddRepresentingCompanyHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            var dbContextHelper = new DbContextHelper();

            country = new Country(countryId, "UK");

            var countries = dbContextHelper.GetAsyncEnabledDbSet(new List<Country> { country });
            A.CallTo(() => weeeContext.Countries).Returns(countries);

            handler = new AddRepresentingCompanyHandler(
                authorization,
                genericDataAccess,
                weeeContext,
                smallProducerDataAccess);
        }

        [Fact]
        public async Task HandleAsync_WhenDirectRegistrantHasNoAuthorisedRepresentative_ShouldUpdateExistingRegistrant()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var registrantId = Guid.NewGuid();
            var existingDirectRegistrant = A.Fake<DirectRegistrant>();

            A.CallTo(() => existingDirectRegistrant.Id).Returns(registrantId);
            A.CallTo(() => existingDirectRegistrant.AuthorisedRepresentative).Returns(null);
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(organisationId))
                .Returns(existingDirectRegistrant);

            var request = new AddRepresentingCompany(organisationId, GetRepresentingCompanyDetailsViewModel());

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(registrantId);
                A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
                A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).MustNotHaveHappened();
            }
        }

        [Fact]
        public async Task HandleAsync_WhenDirectRegistrantHasAuthorisedRepresentative_ShouldCreateNewRegistrant()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var registrantId = Guid.NewGuid();
            var existingDirectRegistrant = A.Fake<DirectRegistrant>();

            A.CallTo(() => existingDirectRegistrant.Id).Returns(registrantId);
            A.CallTo(() => existingDirectRegistrant.AuthorisedRepresentative).Returns(A.Fake<AuthorisedRepresentative>());
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(organisationId))
                .Returns(existingDirectRegistrant);
            A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._))
                .Returns(existingDirectRegistrant);

            var request = new AddRepresentingCompany(organisationId, GetRepresentingCompanyDetailsViewModel());

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(registrantId);
                A.CallTo(() => weeeContext.SaveChangesAsync()).MustNotHaveHappened();
                A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task HandleAsync_WhenValidRequestIsProvidedAndHasAuthorisedRepresentative_ShouldCreateDirectRegistrantAndReturnItsId(
            bool includeBrandName,
            bool includeAdditionalDetails)
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var directRegistrant = A.Fake<DirectRegistrant>();
            var organisation = A.Fake<Organisation>();
            var address = A.Fake<Address>();
            var contact = A.Fake<Contact>();
            var registrantId = Guid.NewGuid();
            var brandName = includeBrandName ? A.Fake<BrandName>() : null;
            var additionalDetails = includeAdditionalDetails ? A.CollectionOfFake<AdditionalCompanyDetails>(2) : new List<AdditionalCompanyDetails>();

            A.CallTo(() => directRegistrant.Id).Returns(registrantId);
            A.CallTo(() => directRegistrant.Contact).Returns(contact);
            A.CallTo(() => directRegistrant.Address).Returns(address);
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => directRegistrant.BrandName).Returns(brandName);
            A.CallTo(() => directRegistrant.AdditionalCompanyDetails).Returns(additionalDetails);
            A.CallTo(() => directRegistrant.AuthorisedRepresentative).Returns(A.Fake<AuthorisedRepresentative>());

            var representingCompanyDetails = GetRepresentingCompanyDetailsViewModel();

            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(A<Guid>._))
                .Returns(directRegistrant);
            A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).Returns(directRegistrant);

            // Act
            var result = await handler.HandleAsync(new AddRepresentingCompany(organisationId, representingCompanyDetails));

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(registrantId);

                A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(organisationId))
                    .MustHaveHappenedOnceExactly()
                    .Then(A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>.That.Matches(d =>
                        d.Contact == contact &&
                        d.Address == address &&
                        d.BrandName == brandName &&
                        d.AdditionalCompanyDetails.ElementsEqual(additionalDetails) &&
                        d.AuthorisedRepresentative.OverseasProducerTradingName == representingCompanyDetails.BusinessTradingName &&
                        d.AuthorisedRepresentative.OverseasProducerName == representingCompanyDetails.CompanyName &&
                        d.AuthorisedRepresentative.OverseasContact.Address.PrimaryName == representingCompanyDetails.Address.Address1 &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Street == representingCompanyDetails.Address.Address2 &&
                        d.AuthorisedRepresentative.OverseasContact.Address.PostCode == representingCompanyDetails.Address.Postcode &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Town == representingCompanyDetails.Address.TownOrCity &&
                        d.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea == representingCompanyDetails.Address.CountyOrRegion &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Country == country &&
                        d.AuthorisedRepresentative.OverseasContact.Email == representingCompanyDetails.Address.Email &&
                        d.AuthorisedRepresentative.OverseasContact.Telephone == representingCompanyDetails.Address.Telephone))).MustHaveHappenedOnceExactly());
            }
        }

        private RepresentingCompanyDetailsViewModel GetRepresentingCompanyDetailsViewModel()
        {
            var repAddress = TestFixture.Build<RepresentingCompanyAddressData>()
                .With(a => a.CountryId, countryId)
                .Create();

            return TestFixture.Build<RepresentingCompanyDetailsViewModel>()
                .With(a => a.Address, repAddress)
                .Create();
        }
    }
}