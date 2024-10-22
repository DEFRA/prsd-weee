namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
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
    using System.Linq;
    using System.Security;
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
        private readonly Guid userId = Guid.NewGuid();
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
                weeeContext, smallProducerDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorisationCheck_IsCalled()
        {
            // Arrange
            var request = new AddRepresentingCompany(TestFixture.Create<Guid>(),
                                                        GetRepresentingCompanyDetailsViewModel());

            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(A<Guid>._))
                .Returns(A.Fake<DirectRegistrant>());

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_OrganisationAuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new AddRepresentingCompany(TestFixture.Create<Guid>(),
                                                            GetRepresentingCompanyDetailsViewModel());

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(request.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var denyAuthorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            var request = new AddRepresentingCompany(TestFixture.Create<Guid>(),
                TestFixture.Create<RepresentingCompanyDetailsViewModel>());

            var authHandler = new AddRepresentingCompanyHandler(
                denyAuthorization,
                genericDataAccess,
                weeeContext,
                smallProducerDataAccess);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await authHandler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_OrganisationAuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var denyAuthorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            var request = new AddRepresentingCompany(TestFixture.Create<Guid>(),
                TestFixture.Create<RepresentingCompanyDetailsViewModel>());

            var authHandler = new AddRepresentingCompanyHandler(
                denyAuthorization,
                genericDataAccess,
                weeeContext, smallProducerDataAccess);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await authHandler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_NoExistingDirectRegistrantForOrganisation_ThrowsArgumentException()
        {
            // Arrange
            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(A<Guid>._))
                .Returns<DirectRegistrant>(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await handler.HandleAsync(new AddRepresentingCompany(Guid.NewGuid(), GetRepresentingCompanyDetailsViewModel())));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task HandleAsync_WhenValidRequestIsProvided_ShouldCreateDirectRegistrantAndReturnItsId(bool includeBrandName, bool includeAdditionalDetails)
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
                        d.AuthorisedRepresentative.OverseasProducerTradingName ==
                        representingCompanyDetails.BusinessTradingName &&
                        d.AuthorisedRepresentative.OverseasProducerName == representingCompanyDetails.CompanyName &&
                        d.AuthorisedRepresentative.OverseasContact.Address.PrimaryName ==
                        representingCompanyDetails.Address.Address1 &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Street ==
                        representingCompanyDetails.Address.Address2 &&
                        d.AuthorisedRepresentative.OverseasContact.Address.PostCode ==
                        representingCompanyDetails.Address.Postcode &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Town ==
                        representingCompanyDetails.Address.TownOrCity &&
                        d.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea ==
                        representingCompanyDetails.Address.CountyOrRegion &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Country == country &&
                        d.AuthorisedRepresentative.OverseasContact.Email == representingCompanyDetails.Address.Email &&
                        d.AuthorisedRepresentative.OverseasContact.Telephone ==
                        representingCompanyDetails.Address.Telephone))).MustHaveHappenedOnceExactly());
            }
        }

        private RepresentingCompanyDetailsViewModel GetRepresentingCompanyDetailsViewModel()
        {
            var repAddress = TestFixture.Build<RepresentingCompanyAddressData>().With(a => a.CountryId, countryId)
                .Create();

            return TestFixture.Build<RepresentingCompanyDetailsViewModel>()
                .With(a => a.Address, repAddress).Create();
        }
    }
}