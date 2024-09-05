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
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class CompleteOrganisationTransactionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess dataAccess;
        private readonly IJsonSerializer serializer;
        private readonly CompleteOrganisationTransactionHandler handler;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext weeeContext;
        private readonly Guid countryId = Guid.NewGuid();
        private readonly Guid userId = Guid.NewGuid();
        private readonly Country country;
        private const string CompanyName = "Company name";
        private const string TradingName = "Trading name";
        
        public CompleteOrganisationTransactionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            var dbContextHelper = new DbContextHelper();

            country = new Country(countryId, "UK");

            var countries = dbContextHelper.GetAsyncEnabledDbSet(new List<Country> { country });
            A.CallTo(() => weeeContext.Countries).Returns(countries);
            A.CallTo(() => userContext.UserId).Returns(userId);

            handler = new CompleteOrganisationTransactionHandler(
                authorization,
                dataAccess,
                serializer,
                transactionAdapter,
                genericDataAccess,
                weeeContext,
                userContext);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var denyAuthorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            var request = new CompleteOrganisationTransaction();
            var authHandler = new CompleteOrganisationTransactionHandler(
                denyAuthorization,
                dataAccess,
                serializer,
                transactionAdapter,
                genericDataAccess,
                weeeContext,
                userContext);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await authHandler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenNoIncompleteTransaction_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(Task.FromResult<OrganisationTransaction>(null));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenNoIncompleteTransactionExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(Task.FromResult<OrganisationTransaction>(null));

            // Act
            Func<Task> act = async () => await handler.HandleAsync(A.Dummy<CompleteOrganisationTransaction>());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Organisation transaction not found.");
        }

        [Fact]
        public async Task HandleAsync__WhenAddressDataIsNull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var organisationTransaction = A.Fake<OrganisationTransaction>();
            var organisationTransactionData = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.Partnership,
                OrganisationViewModel = new OrganisationViewModel
                {
                    BusinessTradingName = "Test Partnership",
                    CompanyName = "Company Name",
                    Address = null
                    // Address is not set to simulate the null case
                }
            };

            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(Task.FromResult(organisationTransaction));
            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(organisationTransaction.OrganisationJson))
                .Returns(organisationTransactionData);

            // Act
            Func<Task> act = async () => await handler.HandleAsync(A.Dummy<CompleteOrganisationTransaction>());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [ClassData(typeof(OrganisationTestDataGenerator))]
        public async Task HandleAsync_WhenValidRequestIsProvided_ShouldCreateOrganisationAndReturnItsId(
            ExternalOrganisationType externalOrganisationType,
            Domain.Organisation.OrganisationType domainOrganisationType,
            string brandNames,
            YesNoType? authorisedRepresentitive)
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var hasBrandName = !string.IsNullOrWhiteSpace(brandNames);

            var transactionData = SetupValidOrganisationTransaction(externalOrganisationType, brandNames, authorisedRepresentitive);

            var newAddress = A.Fake<Address>();
            var directRegistrant = A.Fake<DirectRegistrant>();
            var newOrganisation = A.Fake<Organisation>();
            A.CallTo(() => newOrganisation.Id).Returns(organisationId);
            A.CallTo(() => directRegistrant.Organisation).Returns(newOrganisation);

            BrandName brandName = null;

            if (hasBrandName)
            {
                brandName = A.Fake<BrandName>();
            }

            A.CallTo(() => genericDataAccess.Add(A<Address>._)).Returns(Task.FromResult(newAddress));
            A.CallTo(() => genericDataAccess.Add(A<BrandName>._)).Returns(Task.FromResult(brandName));
            A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).Returns(Task.FromResult(directRegistrant));

            // Act
            var result = await handler.HandleAsync(A.Dummy<CompleteOrganisationTransaction>());

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(organisationId);

                var callConfig = A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly()
                    .Then(A.CallTo(() => genericDataAccess.Add(A<Address>._)).MustHaveHappenedOnceExactly());

                if (hasBrandName)
                {
                    callConfig.Then(A.CallTo(() => genericDataAccess.Add(A<BrandName>._))
                        .MustHaveHappenedOnceExactly());
                }

                callConfig.Then(A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => dataAccess.CompleteTransactionAsync(A<Organisation>._)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => genericDataAccess.Add(A<OrganisationUser>._)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => transactionAdapter.Commit(null)).MustHaveHappenedOnceExactly());

                A.CallTo(() => genericDataAccess.Add(A<Address>.That.Matches(a => a.Country.Id == countryId)))
                    .MustHaveHappenedOnceExactly();

                if (hasBrandName)
                {
                    A.CallTo(() => genericDataAccess.Add(A<BrandName>.That.Matches(d =>
                            d.Name == brandNames)))
                        .MustHaveHappenedOnceExactly();
                }

                if (authorisedRepresentitive == YesNoType.Yes)
                {
                    A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>.That.Matches(d =>
                        d.Organisation.OrganisationType == domainOrganisationType &&
                        d.Organisation.TradingName == TradingName &&
                        d.Organisation.Name == CompanyName &&
                        d.Organisation.BusinessAddress == newAddress &&
                        d.BrandName == brandName &&
                        d.AuthorisedRepresentative.OverseasProducerTradingName == transactionData.RepresentingCompanyDetailsViewModel.BusinessTradingName &&
                        d.AuthorisedRepresentative.OverseasProducerName == transactionData.RepresentingCompanyDetailsViewModel.CompanyName &&
                        d.AuthorisedRepresentative.OverseasContact.Address.PrimaryName == transactionData.RepresentingCompanyDetailsViewModel.Address.Address1 &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Street == transactionData.RepresentingCompanyDetailsViewModel.Address.Address2 &&
                        d.AuthorisedRepresentative.OverseasContact.Address.PostCode == transactionData.RepresentingCompanyDetailsViewModel.Address.Postcode &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Town == transactionData.RepresentingCompanyDetailsViewModel.Address.TownOrCity &&
                        d.AuthorisedRepresentative.OverseasContact.Address.AdministrativeArea == transactionData.RepresentingCompanyDetailsViewModel.Address.CountyOrRegion &&
                        d.AuthorisedRepresentative.OverseasContact.Address.Country == country &&
                        d.AuthorisedRepresentative.OverseasContact.Email == transactionData.RepresentingCompanyDetailsViewModel.Address.Email &&
                        d.AuthorisedRepresentative.OverseasContact.Telephone == transactionData.RepresentingCompanyDetailsViewModel.Address.Telephone))).MustHaveHappenedOnceExactly();
                }
                else
                {
                    A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>.That.Matches(d =>
                        d.Organisation.OrganisationType == domainOrganisationType &&
                        d.Organisation.TradingName == TradingName &&
                        d.Organisation.Name == CompanyName &&
                        d.Organisation.BusinessAddress == newAddress &&
                        d.BrandName == brandName &&
                        d.Contact.FirstName == transactionData.ContactDetailsViewModel.FirstName &&
                        d.Contact.LastName == transactionData.ContactDetailsViewModel.LastName &&
                        d.Contact.Position == transactionData.ContactDetailsViewModel.Position &&
                        d.Address.Address1 == transactionData.ContactDetailsViewModel.AddressData.Address1 &&
                        d.Address.Address2 == transactionData.ContactDetailsViewModel.AddressData.Address2 &&
                        d.Address.TownOrCity == transactionData.ContactDetailsViewModel.AddressData.TownOrCity &&
                        d.Address.CountyOrRegion == transactionData.ContactDetailsViewModel.AddressData.CountyOrRegion &&
                        d.Address.Postcode == transactionData.ContactDetailsViewModel.AddressData.Postcode &&
                        d.Address.Country == country &&
                        d.Address.Email == transactionData.ContactDetailsViewModel.AddressData.Email &&
                        d.Address.Telephone == transactionData.ContactDetailsViewModel.AddressData.Telephone))).MustHaveHappenedOnceExactly();
                }

                A.CallTo(() =>
                        genericDataAccess.Add(
                            A<OrganisationUser>.That.Matches(o => o.OrganisationId == organisationId && o.UserId == userId.ToString() && o.UserStatus == EA.Weee.Domain.User.UserStatus.Active)))
                    .MustHaveHappenedOnceExactly();

                A.CallTo(() => dataAccess.CompleteTransactionAsync(A<Organisation>.That.Matches(o =>
                    o == newOrganisation)))
                .MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async Task HandleAsync_RollsBackTransactionOnError()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();

            A.CallTo(() => genericDataAccess.Add<Address>(A<Address>._)).Throws<Exception>();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(request));
            A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
        }

        private OrganisationTransactionData SetupValidOrganisationTransaction(
            ExternalOrganisationType organisationType = ExternalOrganisationType.RegisteredCompany,
            string brandNames = null,
            YesNoType? authorisedRepresentative = null)
        {
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(transaction);

            var transactionData = new OrganisationTransactionData
            {
                OrganisationType = organisationType,
            };

            OrganisationViewModel organisationViewModel = null;

            var addressData =
                TestFixture.Build<ExternalAddressData>().With(e => e.CountryId, countryId).Create();

            organisationViewModel = TestFixture.Build<OrganisationViewModel>()
                        .With(m => m.Address, addressData)
                        .With(m => m.EEEBrandNames, brandNames)
                        .With(m => m.CompanyName, CompanyName)
                        .With(m => m.BusinessTradingName, TradingName)
                        .Create();

            var organisationContactAddress = TestFixture.Build<AddressPostcodeRequiredData>().With(o => o.CountryId, country.Id).Create();
            transactionData.ContactDetailsViewModel = TestFixture.Build<ContactDetailsViewModel>().With(r => r.AddressData, organisationContactAddress).Create();

            transactionData.OrganisationViewModel = organisationViewModel;
            transactionData.AuthorisedRepresentative = authorisedRepresentative;
            
            if (authorisedRepresentative == YesNoType.Yes)
            {
                var repAddressData =
                    TestFixture.Build<RepresentingCompanyAddressData>().With(e => e.CountryId, countryId).Create();

                transactionData.RepresentingCompanyDetailsViewModel = TestFixture.Build<RepresentingCompanyDetailsViewModel>()
                    .With(m => m.Address, repAddressData)
                    .Create();
            }
            else
            {
                transactionData.RepresentingCompanyDetailsViewModel = null;
            }

            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(transaction.OrganisationJson))
                .Returns(transactionData);

            return transactionData;
        }
    }
}