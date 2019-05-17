namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
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
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class AddAatfRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<UKCompetentAuthorityData, UKCompetentAuthority> competentAuthorityMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;

        public AddAatfRequestHandlerTests()
        {
            this.authorization = AuthorizationBuilder.CreateUserWithAllRights();
            context = A.Fake<WeeeContext>();
            this.addressMapper = A.Fake<IMap<AatfAddressData, AatfAddress>>();
            this.competentAuthorityMapper = A.Fake<IMap<UKCompetentAuthorityData, UKCompetentAuthority>>();
            this.contactMapper = A.Fake<IMap<AatfContactData, AatfContact>>();
            this.dataAccess = A.Fake<IGenericDataAccess>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            AddAatfRequestHandler handler = new AddAatfRequestHandler(authorization, context, this.dataAccess, addressMapper, competentAuthorityMapper, contactMapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatf>());

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

            AddAatfRequestHandler handler = new AddAatfRequestHandler(authorization, context, this.dataAccess, addressMapper, competentAuthorityMapper, contactMapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ValidInput_AddsAatf()
        {
            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();

            AatfData aatf = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now);

            Guid aatfId = Guid.NewGuid();

            AddAatfRequestHandler handler = new AddAatfRequestHandler(authorization, context, this.dataAccess, addressMapper, competentAuthorityMapper, contactMapper);

            AddAatf request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid()
            };

            A.CallTo(() => dataAccess.Add<Domain.AatfReturn.Aatf>(A<Domain.AatfReturn.Aatf>.That.Matches(c => c.Name == aatf.Name))).Returns(aatfId);
            bool result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Domain.AatfReturn.Aatf>(A<Domain.AatfReturn.Aatf>.That.Matches(c => c.Name == aatf.Name))).MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));

            result.Should().Be(true);
        }
    }
}
