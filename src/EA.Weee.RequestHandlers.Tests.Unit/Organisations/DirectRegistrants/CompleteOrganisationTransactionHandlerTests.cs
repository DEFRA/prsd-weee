namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Weee.Core.Helpers;
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
    using System.Data.Entity;
    using System.Linq.Expressions;
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
        private readonly Guid countryId = Guid.NewGuid();
        private const string CompanyName = "Company name";
        private const string TradingName = "Trading name";
        private const string BrandNames = "Brand names";

        public CompleteOrganisationTransactionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            var weeeContext = A.Fake<WeeeContext>();

            var dbContextHelper = new DbContextHelper();

            var country = new Country(countryId, "UK");

            var countries = dbContextHelper.GetAsyncEnabledDbSet(new List<Country> { country });
            A.CallTo(() => weeeContext.Countries).Returns(countries);

            handler = new CompleteOrganisationTransactionHandler(
                authorization,
                dataAccess,
                serializer,
                transactionAdapter,
                genericDataAccess,
                weeeContext);
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
        public async Task HandleAsync_NoIncompleteTransaction_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(Task.FromResult<OrganisationTransaction>(null));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenNoIncompleteTransactionExists()
        {
            // Arrange
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(Task.FromResult<OrganisationTransaction>(null));

            // Act
            Func<Task> act = async () => await handler.HandleAsync(A.Dummy<CompleteOrganisationTransaction>());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("CompleteOrganisationTransactionHandler address null");
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenAddressDataIsNull()
        {
            // Arrange
            var organisationTransaction = A.Fake<OrganisationTransaction>();
            var organisationTransactionData = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.Partnership,
                PartnershipDetailsViewModel = new PartnershipDetailsViewModel
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

        public static IEnumerable<object[]> OrganisationValues()
        {
            yield return new object[] { ExternalOrganisationType.RegisteredCompany, Domain.Organisation.OrganisationType.RegisteredCompany, BrandNames };
            yield return new object[] { ExternalOrganisationType.Partnership, Domain.Organisation.OrganisationType.Partnership, BrandNames };
            yield return new object[] { ExternalOrganisationType.SoleTrader, Domain.Organisation.OrganisationType.SoleTraderOrIndividual, BrandNames };
            yield return new object[] { ExternalOrganisationType.RegisteredCompany, Domain.Organisation.OrganisationType.RegisteredCompany, null };
            yield return new object[] { ExternalOrganisationType.Partnership, Domain.Organisation.OrganisationType.Partnership, null };
            yield return new object[] { ExternalOrganisationType.SoleTrader, Domain.Organisation.OrganisationType.SoleTraderOrIndividual, null };
        }

        [Theory]
        [MemberData(nameof(OrganisationValues))]
        public async Task HandleAsync_WhenValidRequestIsProvided_ShouldCreateOrganisationAndReturnItsId(ExternalOrganisationType externalOrganisationType,
            Domain.Organisation.OrganisationType domainOrganisationType, string brandNames)
        {
            // Arrange
            var organisationId = Guid.NewGuid();

            SetupValidOrganisationTransaction(externalOrganisationType, brandNames);

            var newAddress = A.Fake<Address>();
            var newOrganisation = A.Fake<Organisation>();
            A.CallTo(() => newOrganisation.Id).Returns(organisationId);

            A.CallTo(() => genericDataAccess.Add(A<Address>._)).Returns(Task.FromResult(newAddress));
            A.CallTo(() => genericDataAccess.Add(A<Organisation>._)).Returns(Task.FromResult(newOrganisation));

            // Act
            var result = await handler.HandleAsync(A.Dummy<CompleteOrganisationTransaction>());

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(organisationId);

                var callConfig = A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly()
                    .Then(A.CallTo(() => genericDataAccess.Add(A<Address>._)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => genericDataAccess.Add(A<Organisation>._)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).MustHaveHappenedOnceExactly());

                callConfig.Then(A.CallTo(() => dataAccess.CompleteTransactionAsync(A<Organisation>._)).MustHaveHappenedOnceExactly())
                    .Then(A.CallTo(() => transactionAdapter.Commit(null)).MustHaveHappenedOnceExactly());

                A.CallTo(() => genericDataAccess.Add(A<Address>.That.Matches(a => a.Country.Id == countryId)))
                    .MustHaveHappenedOnceExactly();

                A.CallTo(() => genericDataAccess.Add(A<Organisation>.That.Matches(o =>
                    o.OrganisationType == domainOrganisationType &&
                    o.Name == CompanyName &&
                    o.TradingName == TradingName &&
                    o.BusinessAddress == newAddress)))
                .MustHaveHappenedOnceExactly();

                A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>.That.Matches(d =>
                    d.Organisation == newOrganisation)))
                .MustHaveHappenedOnceExactly();

                A.CallTo(() => dataAccess.CompleteTransactionAsync(A<Organisation>.That.Matches(o =>
                    o == newOrganisation)))
                .MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async Task HandleAsync_AddsAddressToOrganisation()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => genericDataAccess.Add(A<Address>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CreatesBrandNameIfProvided()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction(brandNames: "TestBrand");

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => genericDataAccess.Add(A<BrandName>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CreatesDirectRegistrant()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => genericDataAccess.Add(A<DirectRegistrant>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CompletesTransaction()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.CompleteTransactionAsync(A<Organisation>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CommitsTransaction()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();
            
            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Commit(null)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_RollsBackTransactionOnError()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction();
            SetupValidOrganisationTransaction();
            
            A.CallTo(() => genericDataAccess.Add(A<Organisation>._)).Throws<Exception>();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(request));
            A.CallTo(() => transactionAdapter.BeginTransaction()).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionAdapter.Rollback(null)).MustHaveHappenedOnceExactly();
        }

        private OrganisationTransactionData SetupValidOrganisationTransaction(
            ExternalOrganisationType organisationType = ExternalOrganisationType.RegisteredCompany,
            string brandNames = null)
        {
            var transaction = new OrganisationTransaction { OrganisationJson = "{}" };
            A.CallTo(() => dataAccess.FindIncompleteTransactionForCurrentUserAsync())
                .Returns(transaction);

            var transactionData = new OrganisationTransactionData
            {
                OrganisationType = organisationType,
            };

            RegisteredCompanyDetailsViewModel registeredCompanyDetailsView = null;
            PartnershipDetailsViewModel partnershipDetailsViewModel = null;
            SoleTraderDetailsViewModel soleTraderDetailsViewModel = null;
            var addressData =
                TestFixture.Build<ExternalAddressData>().With(e => e.CountryId, countryId).Create();
            switch (organisationType)
            {
                case ExternalOrganisationType.RegisteredCompany:
                    registeredCompanyDetailsView = TestFixture.Build<RegisteredCompanyDetailsViewModel>()
                        .With(m => m.Address, addressData)
                        .With(m => m.EEEBrandNames, brandNames)
                        .With(m => m.CompanyName, CompanyName)
                        .With(m => m.BusinessTradingName, TradingName)
                        .Create();
                    break;
                case ExternalOrganisationType.Partnership:
                    partnershipDetailsViewModel = TestFixture.Build<PartnershipDetailsViewModel>().With(m => m.Address, addressData)
                        .With(m => m.CompanyName, CompanyName)
                        .With(m => m.BusinessTradingName, TradingName)
                        .With(m => m.EEEBrandNames, brandNames).Create();
                    break;
                case ExternalOrganisationType.SoleTrader:
                    soleTraderDetailsViewModel = TestFixture.Build<SoleTraderDetailsViewModel>().With(m => m.Address, addressData)
                        .With(m => m.CompanyName, CompanyName)
                        .With(m => m.BusinessTradingName, TradingName)
                        .With(m => m.EEEBrandNames, brandNames).Create();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(organisationType), organisationType, null);
            }

            transactionData.RegisteredCompanyDetailsViewModel = registeredCompanyDetailsView;
            transactionData.PartnershipDetailsViewModel = partnershipDetailsViewModel;
            transactionData.SoleTraderDetailsViewModel = soleTraderDetailsViewModel;

            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(transaction.OrganisationJson))
                .Returns(transactionData);

            return transactionData;
        }
    }
}