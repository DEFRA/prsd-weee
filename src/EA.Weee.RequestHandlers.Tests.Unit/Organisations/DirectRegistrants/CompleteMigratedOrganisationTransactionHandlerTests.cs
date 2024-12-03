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
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using OrganisationStatus = Domain.Organisation.OrganisationStatus;
    using OrganisationType = Domain.Organisation.OrganisationType;

    public class CompleteMigratedOrganisationTransactionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess dataAccess;
        private readonly IJsonSerializer serializer;
        private readonly CompleteMigratedOrganisationTransactionHandler handler;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext weeeContext;
        private readonly Guid countryId = Guid.NewGuid();
        private readonly Guid userId = Guid.NewGuid();
        private readonly Country country;
        private const string CompanyName = "Company name";
        private const string TradingName = "Trading name";
        private const string CompanyRegNumber = "12345678";

        public CompleteMigratedOrganisationTransactionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();

            country = new Country(countryId, "UK");
            var dbContextHelper = new DbContextHelper();
            var countries = dbContextHelper.GetAsyncEnabledDbSet(new[] { country });

            A.CallTo(() => weeeContext.Countries).Returns(countries);
            A.CallTo(() => userContext.UserId).Returns(userId);

            handler = new CompleteMigratedOrganisationTransactionHandler(
                authorization, dataAccess, serializer, transactionAdapter,
                genericDataAccess, weeeContext, userContext);
        }

        [Fact]
        public async Task HandleAsync_NoAuthorization_ThrowsSecurityException()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            A.CallTo(() => authorization.EnsureCanAccessExternalArea())
                .Throws(new SecurityException());

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_NoDirectRegistrantId_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };

            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId))
                .Returns((DirectRegistrant)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_NotANpWdMigratedOrganisation_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };

            var directRegistrant = A.Fake<DirectRegistrant>();
            var organisation = Organisation.CreateRegisteredCompany("Company", "12345456");
            organisation.SetNpwdMigrated(false);

            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);

            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId))
                .Returns(directRegistrant);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_NoIncompleteTransaction_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns((OrganisationTransaction)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_UpdatesOrganisationDetails_Correctly()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };
            var organisation = Organisation.CreateRegisteredCompany(CompanyName, CompanyRegNumber, TradingName);
            organisation.SetNpwdMigrated(true);
            var directRegistrant = A.Fake<DirectRegistrant>();

            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId)).Returns(directRegistrant);

            var transactionData = SetupValidOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            organisation.Name.Should().Be(CompanyName);
            organisation.TradingName.Should().Be(TradingName);
            organisation.OrganisationStatus.Should().Be(OrganisationStatus.Incomplete); // organisation status will be set during migration
            organisation.NpwdMigratedComplete.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_WhenCompletingMigration_UpdatesOrganisationStatus()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var organisation = Organisation.CreateRegisteredCompany(CompanyName, CompanyRegNumber, TradingName);
            organisation.SetNpwdMigrated(true);
            var directRegistrant = A.Fake<DirectRegistrant>();
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };

            organisation.NpwdMigratedComplete = false;
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId)).Returns(directRegistrant);

            SetupValidOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            organisation.NpwdMigratedComplete.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_WhenAlreadyMigrated_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var organisation = Organisation.CreateRegisteredCompany(CompanyName, CompanyRegNumber, TradingName);
            var directRegistrant = A.Fake<DirectRegistrant>();
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };

            organisation.NpwdMigratedComplete = true;
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId)).Returns(directRegistrant);

            SetupValidOrganisationTransaction();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_UpdatesOrganisationType_AndMaintainsRegistrationNumber()
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var organisation = Organisation.CreateRegisteredCompany(CompanyName, CompanyRegNumber, TradingName);
            organisation.SetNpwdMigrated(true);
            var directRegistrant = A.Fake<DirectRegistrant>();
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };

            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId)).Returns(directRegistrant);

            var transactionData = SetupValidOrganisationTransaction(ExternalOrganisationType.RegisteredCompany);

            // Act
            await handler.HandleAsync(request);

            // Assert
            organisation.OrganisationType.Should().Be(OrganisationType.RegisteredCompany);
            organisation.CompanyRegistrationNumber.Should().Be(CompanyRegNumber);
        }

        [Theory]
        [InlineData(ExternalOrganisationType.Partnership, "Trading Name")]
        [InlineData(ExternalOrganisationType.SoleTrader, "Company Name")]
        [InlineData(ExternalOrganisationType.RegisteredCompany, "Company Name")]
        public async Task HandleAsync_SetsCorrectOrganisationName_BasedOnType(ExternalOrganisationType type, string expectedName)
        {
            // Arrange
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());
            var organisation = Organisation.CreateRegisteredCompany(CompanyName, CompanyRegNumber, TradingName);
            organisation.SetNpwdMigrated(true);
            var directRegistrant = A.Fake<DirectRegistrant>();
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };

            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(transaction);
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId)).Returns(directRegistrant);

            var transactionData = SetupValidOrganisationTransaction(type, companyName: expectedName);

            // Act
            await handler.HandleAsync(request);

            // Assert
            organisation.OrganisationName.Should().Be(expectedName);
        }

        private Address CreateAddress()
        {
            return new Address(
                "123 Test St",
                "Test Suite",
                "Test City",
                "Test Region",
                "TE12 3ST",
                country,
                "01234567890",
                "test@test.com");
        }

        [Theory]
        [ClassData(typeof(OrganisationTestDataGenerator))]
        public async Task HandleAsync_WhenValidRequestIsProvided_ShouldUpdateOrganisationAndReturnItsId(
            ExternalOrganisationType externalOrganisationType,
            OrganisationType domainOrganisationType,
            string brandNames,
            YesNoType? authorisedRepresentative)
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var hasBrandName = !string.IsNullOrWhiteSpace(brandNames);
            var request = new CompleteMigratedOrganisationTransaction(Guid.NewGuid());

            var transactionData =
                SetupValidOrganisationTransaction(externalOrganisationType, brandNames, authorisedRepresentative);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var organisation = Organisation.CreateRegisteredCompany("Company Name", "12345678", "Trading Name");
            organisation.UpdateMigratedOrganisationType(domainOrganisationType);
            organisation.SetNpwdMigrated(true);

            var address = CreateAddress();
            var contact = A.Fake<Contact>();
            var brandName = hasBrandName ? A.Fake<BrandName>() : null;

            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisationId);
            A.CallTo(() => directRegistrant.Contact).Returns(contact);
            A.CallTo(() => directRegistrant.Address).Returns(address);
            A.CallTo(() => directRegistrant.BrandName).Returns(brandName);
            A.CallTo(() => directRegistrant.AuthorisedRepresentative).Returns(authorisedRepresentative == YesNoType.Yes
                ? A.Fake<AuthorisedRepresentative>()
                : null);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId))
                .Returns(directRegistrant);
            A.CallTo(() => genericDataAccess.Add(A<Address>._)).Returns(address);
            A.CallTo(() => genericDataAccess.Add(A<BrandName>._)).Returns(brandName);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(organisationId);

                var callConfig = A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly()
                    .Then(A.CallTo(() => genericDataAccess.Add(A<Address>._)).MustHaveHappenedOnceExactly());

                // Organisation updates
                organisation.NpwdMigrated.Should().BeTrue();

                A.CallTo(() => genericDataAccess.Add(A<Address>.That.Matches(a => a.Country.Id == countryId)))
                    .MustHaveHappenedOnceExactly();

                organisation.IsRepresentingCompany.Should().Be(authorisedRepresentative == YesNoType.Yes);
                organisation.NpwdMigratedComplete.Should().BeTrue();

                // Brand name handling
                if (hasBrandName)
                {
                    A.CallTo(() => genericDataAccess.Add(A<BrandName>.That.Matches(d => d.Name == brandNames)))
                        .MustHaveHappenedOnceExactly();
                }

                // Contact and address updates
                A.CallTo(() => directRegistrant.AddOrUpdateMainContactPerson(A<Contact>.That.Matches(c =>
                    c.FirstName == transactionData.ContactDetailsViewModel.FirstName &&
                    c.LastName == transactionData.ContactDetailsViewModel.LastName &&
                    c.Position == transactionData.ContactDetailsViewModel.Position)))
                    .MustHaveHappenedOnceExactly();

                A.CallTo(() => directRegistrant.AddOrUpdateAddress(A<Address>.That.Matches(a =>
                    a.Address1 == transactionData.ContactDetailsViewModel.AddressData.Address1 &&
                    a.Address2 == transactionData.ContactDetailsViewModel.AddressData.Address2 &&
                    a.TownOrCity == transactionData.ContactDetailsViewModel.AddressData.TownOrCity &&
                    a.CountyOrRegion == transactionData.ContactDetailsViewModel.AddressData.CountyOrRegion &&
                    a.Postcode == transactionData.ContactDetailsViewModel.AddressData.Postcode &&
                    a.Country == country &&
                    a.Email == transactionData.ContactDetailsViewModel.AddressData.Email &&
                    a.Telephone == transactionData.ContactDetailsViewModel.AddressData.Telephone)))
                    .MustHaveHappenedOnceExactly();

                // Authorised representative
                if (authorisedRepresentative == YesNoType.Yes)
                {
                    A.CallTo(() => directRegistrant.AddOrUpdateAuthorisedRepresentitive(A<AuthorisedRepresentative>.That.Matches(ar =>
                        ar.OverseasProducerTradingName == transactionData.RepresentingCompanyDetailsViewModel.BusinessTradingName &&
                        ar.OverseasProducerName == transactionData.RepresentingCompanyDetailsViewModel.CompanyName &&
                        ar.OverseasContact.Address.PrimaryName == transactionData.RepresentingCompanyDetailsViewModel.Address.Address1 &&
                        ar.OverseasContact.Address.Street == transactionData.RepresentingCompanyDetailsViewModel.Address.Address2 &&
                        ar.OverseasContact.Address.PostCode == transactionData.RepresentingCompanyDetailsViewModel.Address.Postcode &&
                        ar.OverseasContact.Address.Town == transactionData.RepresentingCompanyDetailsViewModel.Address.TownOrCity &&
                        ar.OverseasContact.Address.AdministrativeArea == transactionData.RepresentingCompanyDetailsViewModel.Address.CountyOrRegion &&
                        ar.OverseasContact.Address.Country == country &&
                        ar.OverseasContact.Email == transactionData.RepresentingCompanyDetailsViewModel.Address.Email &&
                        ar.OverseasContact.Telephone == transactionData.RepresentingCompanyDetailsViewModel.Address.Telephone)))
                        .MustHaveHappenedOnceExactly();
                }

                // Additional company details
                if (transactionData.PartnerModels != null)
                {
                    A.CallTo(() => directRegistrant.SetAdditionalCompanyDetails(A<List<AdditionalCompanyDetails>>.That.Matches(acd =>
                        acd.Count == transactionData.PartnerModels.Count &&
                        acd.All(d => d.Type == OrganisationAdditionalDetailsType.Partner) &&
                        acd.SequenceEqual(transactionData.PartnerModels.OrderBy(p => p.Order).Select(p =>
                            new AdditionalCompanyDetails
                            {
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                Type = OrganisationAdditionalDetailsType.Partner,
                                Order = p.Order
                            })))))
                        .MustHaveHappenedOnceExactly();
                }
                else if (transactionData.SoleTraderViewModel != null)
                {
                    A.CallTo(() => directRegistrant.SetAdditionalCompanyDetails(A<List<AdditionalCompanyDetails>>.That.Matches(acd =>
                        acd.Count == 1 &&
                        acd[0].FirstName == transactionData.SoleTraderViewModel.FirstName &&
                        acd[0].LastName == transactionData.SoleTraderViewModel.LastName &&
                        acd[0].Type == OrganisationAdditionalDetailsType.SoleTrader)))
                        .MustHaveHappenedOnceExactly();
                }

                // Final transaction steps
                A.CallTo(() => dataAccess.CompleteTransactionAsync(organisation))
                    .MustHaveHappenedOnceExactly();

                A.CallTo(() => genericDataAccess.Add(A<OrganisationUser>.That.Matches(o =>
                    o.OrganisationId == organisationId &&
                    o.UserId == userId.ToString() &&
                    o.UserStatus == Domain.User.UserStatus.Active)))
                    .MustHaveHappenedOnceExactly();

                callConfig.Then(A.CallTo(() => transactionAdapter.Commit(null)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly());
            }
        }

        private OrganisationTransactionData SetupValidOrganisationTransaction(
            ExternalOrganisationType organisationType = ExternalOrganisationType.RegisteredCompany,
            string brandNames = null,
            YesNoType? authorisedRepresentative = null,
            string companyName = CompanyName,
            string businessTradingName = TradingName,
            ExternalAddressData addressData = null)
        {
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(transaction);

            if (addressData == null)
            {
                addressData = TestFixture.Build<ExternalAddressData>()
                    .With(e => e.CountryId, countryId)
                    .Create();
            }

            var transactionData = new OrganisationTransactionData
            {
                OrganisationType = organisationType,
                OrganisationViewModel = new OrganisationViewModel
                {
                    CompanyName = companyName,
                    BusinessTradingName = businessTradingName,
                    CompaniesRegistrationNumber = CompanyRegNumber,
                    Address = addressData,
                    EEEBrandNames = brandNames
                },
                ContactDetailsViewModel = TestFixture.Build<ContactDetailsViewModel>()
                    .With(r => r.AddressData, TestFixture.Build<AddressPostcodeRequiredData>()
                        .With(o => o.CountryId, country.Id)
                        .Create())
                    .Create(),
                AuthorisedRepresentative = authorisedRepresentative
            };

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