namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class CreateOrganisationAdminHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly WeeeContext context;

        private readonly Guid countryId = Guid.NewGuid();

        public CreateOrganisationAdminHandlerTests()
        {
            this.authorization = AuthorizationBuilder.CreateUserWithAllRights();
            this.dataAccess = A.Fake<IGenericDataAccess>();
            this.context = A.Fake<WeeeContext>();

            DbContextHelper dbContextHelper = new DbContextHelper();

            Country country = new Country(countryId, "France");

            DbSet<Country> countries = dbContextHelper.GetAsyncEnabledDbSet(new List<Country> { country });
            A.CallTo(() => context.Countries).Returns(countries);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CreateOrganisationAdmin>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CreateOrganisationAdmin>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_Partnership_CreatesOrganisation()
        {
            CreateOrganisationAdmin request = new CreateOrganisationAdmin()
            {
                BusinessName = "Business",
                Address = CreateAddress(),
                OrganisationType = Core.Organisations.OrganisationType.Partnership
            };

            var organisation = A.Fake<Organisation>();
            var organisationId = Guid.NewGuid();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName))).Returns(organisation);

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.Add<Address>(A<Address>.That.Matches(p => p.Address1 == request.Address.Address1))).MustHaveHappened(1, Times.Exactly);

            result.Should().Be(organisationId);
        }

        [Fact]
        public async Task HandleAsync_SoleTraderOrIndividual_CreatesOrganisation()
        {
            CreateOrganisationAdmin request = new CreateOrganisationAdmin()
            {
                BusinessName = "Business",
                Address = CreateAddress(),
                OrganisationType = Core.Organisations.OrganisationType.SoleTraderOrIndividual
            };

            var organisation = A.Fake<Organisation>();
            var organisationId = Guid.NewGuid();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName))).Returns(organisation);

            Guid result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.Add<Address>(A<Address>.That.Matches(p => p.Address1 == request.Address.Address1))).MustHaveHappened(1, Times.Exactly);

            result.Should().Be(organisationId);
        }

        [Fact]
        public async Task HandleAsync_SoleTraderOrIndividualWithOptionalBusinessName_CreatesOrganisation()
        {
            CreateOrganisationAdmin request = new CreateOrganisationAdmin()
            {
                BusinessName = "Business",
                Address = CreateAddress(),
                OrganisationType = Core.Organisations.OrganisationType.SoleTraderOrIndividual,
                TradingName = "Trading"
            };

            var organisation = A.Fake<Organisation>();
            var organisationId = Guid.NewGuid();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName && p.TradingName == request.TradingName))).Returns(organisation);

            Guid result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.Add<Address>(A<Address>.That.Matches(p => p.Address1 == request.Address.Address1))).MustHaveHappened(1, Times.Exactly);

            result.Should().Be(organisationId);
        }

        [Fact]
        public async Task HandleAsync_RegisteredCompany_CreatesOrganisation()
        {
            CreateOrganisationAdmin request = new CreateOrganisationAdmin()
            {
                BusinessName = "Business",
                Address = CreateAddress(),
                OrganisationType = Core.Organisations.OrganisationType.RegisteredCompany,
                RegistrationNumber = "1234567",
                TradingName = "Trading"
            };

            var organisation = A.Fake<Organisation>();
            var organisationId = Guid.NewGuid();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName && p.TradingName == request.TradingName))).Returns(organisation);

            Guid result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Organisation>(A<Organisation>.That.Matches(p => p.OrganisationName == request.BusinessName && p.TradingName == request.TradingName))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => dataAccess.Add<Address>(A<Address>.That.Matches(p => p.Address1 == request.Address.Address1))).MustHaveHappened(1, Times.Exactly);

            result.Should().Be(organisationId);
        }

        [Fact]
        public async Task HandleAsync_OrganisationTypeNotSet_ThrowsNotImplementedException()
        {
            CreateOrganisationAdmin request = new CreateOrganisationAdmin()
            {
                BusinessName = "Business",
                Address = A.Dummy<AddressData>()
            };

            Guid organisationId = Guid.NewGuid();

            CreateOrganisationAdminHandler handler = new CreateOrganisationAdminHandler(authorization, this.dataAccess, this.context);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<NotImplementedException>(action);
        }

        private AddressData CreateAddress()
        {
            return new AddressData()
            {
                Address1 = "1",
                Address2 = "2",
                TownOrCity = "town",
                CountyOrRegion = "county",
                Postcode = "postcode",
                CountryName = "france",
                CountryId = countryId,
                Telephone = "01225 687 698",
                Email = "email@email.com"
            };
        }
    }
}
